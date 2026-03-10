using SAFQA.BLL.Managers.AccountManager.Email_Sender;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SAFQA.DAL.Dtos.AccountDto.User;
using SAFQA.BLL.Enums;
using SAFQA.BLL.Help;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;



namespace SAFQA.BLL.Managers.AccountManager.Auth
{
    public class AuthUser : IAuthUser
    {
        #region  Dependency Injection , UserManagement & SignInManager in Identity , IConfiguration To Access To App Settings 
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly SAFQA_Context _context;

        public AuthUser(UserManager<User> userManager
            , SignInManager<User> signInManager
            , IConfiguration configuration
            , IEmailSender emailSender
            , SAFQA_Context context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _context = context;
        }
        #endregion

        #region  check By Email , Create User Object , Assign Password To This Email , Add Role To User By Identity
        public async Task<AuthResult> RegisterAsync(RegisterDto dto , string deviceId)
        {
            // التأكد من أن الإيميل مش موجود بالفعل
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new() { "Email already in use" }
                };

            // التأكد لو في pending user موجود
            var pendingUser = await _context.PendingUserRegistrations
                .FirstOrDefaultAsync(x => x.Email == dto.Email && !x.IsUsed);

            if (pendingUser != null)
            {
                // إذا صلاحية OTP انتهت نقدر نعيد إرسال OTP
                if (pendingUser.OtpExpiration < DateTime.UtcNow)
                {
                    _context.PendingUserRegistrations.Remove(pendingUser);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Errors = new() { "OTP already sent. Please check your email." }
                    };
                }
            }

            // توليد OTP
            var otp = Helper.GenerateOtp();

            // أخذ secret ومدة انتهاء صلاحية OTP من config
            string otpSecret = _configuration["Security:OtpSecret"];
            int otpExpiryMinutes = 5; // default
            var otpExpiryConfig = _configuration["Security:OtpExpiryMinutes"];
            if (!string.IsNullOrEmpty(otpExpiryConfig))
                otpExpiryMinutes = int.Parse(otpExpiryConfig);

            // إنشاء PendingUserRegistration مع تخزين plain password مؤقتًا
            var pending = new PendingUserRegistration
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = dto.Password.Hash(),
                PhoneNumber = dto.PhoneNumber,
                Gender = dto.Gender,
                BirthDate = dto.BirthDate,
                CityId = dto.cityId,
                OtpHash = Helper.HashOtp(otp, otpSecret),
                OtpExpiration = DateTime.UtcNow.AddMinutes(otpExpiryMinutes),
                IsUsed = false
            };

            _context.PendingUserRegistrations.Add(pending);
            await _context.SaveChangesAsync();

            // إرسال OTP
            await _emailSender.SendOtpEmailAsync(dto.Email, otp);

            return new AuthResult
            {
                IsSuccess = true,
                Message = "OTP sent to your email"
            };
        }
        #endregion

        #region Confirm Email and Create User
        public async Task<AuthResult> ConfirmEmailAsync(ConfirmEmailDto dto)
        {
            // استرجاع pending user
            var pending = await _context.PendingUserRegistrations
                .Where(x => x.Email == dto.Email && !x.IsUsed && x.OtpExpiration > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (pending == null)
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new() { "Invalid or expired OTP" }
                };

            // التحقق من OTP
            var otpHash = Helper.HashOtp(dto.Otp, _configuration["Security:OtpSecret"]);
            if (otpHash != pending.OtpHash)
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new() { "Invalid OTP" }
                };

            // إنشاء المستخدم النهائي باستخدام plain password
            var user = new User
            {
                FullName = pending.FullName,
                Email = pending.Email,
                UserName = pending.Email,
                PhoneNumber = pending.PhoneNumber,
                Gender = pending.Gender,
                BirthDate = pending.BirthDate,
                CityId = pending.CityId,
                Status = UserStatus.Active,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                IsProfileCompleted = false,
                IsDeleted = false
            };

            var result = await _userManager.CreateAsync(user, pending.PasswordHash);
            if (!result.Succeeded)
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };

            // تعليم pending user انه تم استخدامه وحذف الـ plain password
            pending.IsUsed = true;
            await _context.SaveChangesAsync();

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Email confirmed and account created successfully"
            };
        }
        #endregion

        #region  Search By Email , Check Password To This Email , Generate Token
        public async Task<AuthResult> LoginAsync(LoginDto dto , string deviceId)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid login attempt" }
                };
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid login attempt" }
                };
            }
            var token = await GenerateTokensAsync(user , deviceId);
            return new AuthResult
            {
                IsSuccess = true,
                UserId = user.Id,
                Token = token.Token,
                RefreshToken = token.RefreshToken
            };
        }
        #endregion

        #region Search User. Token That == Argument Token , Check For This UsrToken ,Generate Token By Refresh Token & Add ExpiryDate
        public async Task<AuthResult> RefreshTokenAsync(string refreshToken, string deviceId)
        {
            var tokenhash = refreshToken.Hash();
            var storedToken = await _context.refreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r =>
                    r.TokenHash == tokenhash &&
                    r.DeviceId == deviceId &&
                    !r.IsRevoked);

            if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
                return new AuthResult { IsSuccess = false, Message = "Invalid or expired refresh token" };

            storedToken.IsRevoked = true;

            var tokens = await GenerateTokensAsync(storedToken.User,deviceId);

            await _context.SaveChangesAsync();

            return new AuthResult
            {
                IsSuccess = true,
                UserId = storedToken.User.Id,
                Token = tokens.Token,
                RefreshToken = tokens.RefreshToken
            };
        }
        #endregion

        #region Generate Tokens
        private async Task<(string Token, string RefreshToken)> GenerateTokensAsync(User user , string deviceId)
        {
            var claims = new List<Claim>
        {
            new Claim("SecurityStamp", user.SecurityStamp),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddMinutes(30),
                claims: claims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshTokenHash = refreshToken.Hash();


            _context.refreshTokens.Add(new RefreshToken
            {
                TokenHash = refreshTokenHash,
                UserId = user.Id,
                DeviceId = deviceId,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });
            await _context.SaveChangesAsync();

            return (new JwtSecurityTokenHandler().WriteToken(token), refreshToken);
        }
        #endregion

        public async Task<AuthResult> ResendRegistrationOtpAsync(string email)
        {
            // نجيب ال pending user
            var pendingUser = await _context.PendingUserRegistrations
                .FirstOrDefaultAsync(u => u.Email == email);

            if (pendingUser == null)
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Pending user not found"
                };

            // cooldown بالدقيقة (60 ثانية)
            var cooldown = int.Parse(_configuration["Security:OtpCooldownSeconds"]);

            // تحقق من وقت آخر ارسال OTP
            if (pendingUser.LastOtpSentAt.HasValue &&
                pendingUser.LastOtpSentAt.Value.AddSeconds(cooldown) > DateTime.UtcNow)
            {
                var waitTime = (pendingUser.LastOtpSentAt.Value.AddSeconds(cooldown) - DateTime.UtcNow).Seconds;
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = $"Please wait {waitTime} seconds before requesting another OTP"
                };
            }

            // generate new OTP
            var code = Helper.GenerateOtp();
            var hash = Helper.HashOtp(code, _configuration["Security:OtpSecret"]);

            // update pending user OTP
            pendingUser.OtpHash = hash;
            pendingUser.OtpExpiration = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Security:OtpExpiryMinutes"]));
            pendingUser.IsUsed = false;
            pendingUser.LastOtpSentAt = DateTime.UtcNow; // تحديث وقت آخر إرسال

            _context.PendingUserRegistrations.Update(pendingUser);
            await _context.SaveChangesAsync();

            // ارسال الايميل
            await _emailSender.SendEmailAsync(
                pendingUser.Email,
                "Your Registration OTP",
                $"<h2>Your OTP is: {code}</h2>");

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Registration OTP resent successfully"
            };
        }
    }
}
