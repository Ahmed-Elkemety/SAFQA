using SAFQA.BLL.Managers.AccountManager.Email_Sender;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SAFQA.BLL.Dtos.AccountDto.User;
using SAFQA.BLL.Enums;
using SAFQA.BLL.Help;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using SAFQA.BLL.Dtos.AccountDto.Forget_password;



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
                PasswordHash = dto.Password,
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
        public async Task<AuthResult> LoginAsync(LoginDto dto, string deviceId)
        {
            // 1️⃣ نبحث في جدول Users
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                // 2️⃣ لو مش موجود، نشوف جدول PendingUserRegistrations
                var pendingUser = await _context.PendingUserRegistrations
                    .FirstOrDefaultAsync(p => p.Email == dto.Email);

                if (pendingUser != null)
                {
                    // Email مسجل لكن لم يتم التحقق من OTP
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Email registered but OTP not verified. Please verify your email." }
                    };
                }

                // Email مش موجود نهائيًا
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid login attempt" }
                };
            }

            // 3️⃣ لو المستخدم موجود في Users، نتحقق من كلمة السر
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid login attempt" }
                };
            }

            // 4️⃣ Generate Tokens
            var token = await GenerateTokensAsync(user, deviceId);
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
        public async Task<AuthResult> RequestPasswordResetAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new AuthResult { IsSuccess = false, Message = "User not found" };

            // Cleanup أي OTPs قديمة
            await CleanupOtpsAsync(user.Id);

            // Generate OTP + hash
            var code = Helper.GenerateOtp();
            var hashed = Helper.HashOtp(code, _configuration["Security:OtpSecret"]);

            // Save new OTP
            _context.PasswordResetOtps.Add(new PasswordResetOtp
            {
                UserId = user.Id,
                CodeHash = hashed,
                Expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Security:OtpExpiryMinutes"])),
                IsUsed = false
            });
            await _context.SaveChangesAsync();

            // Send OTP Email
            var body = $"<h2>Your OTP code is: {code}</h2><p>It expires in 10 minutes.</p>";
            await _emailSender.SendEmailAsync(user.Email, "Password Reset Code", body);

            return new AuthResult { IsSuccess = true, Message = "OTP sent to your email" };
        }

        public async Task<AuthResult> VerifyOtpAsync(VerifyOtpDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "User not found"
                };

            // حذف OTPs القديمة
            await CleanupOtpsAsync(user.Id);

            var record = await _context.PasswordResetOtps
                .Where(x =>
                    x.UserId == user.Id &&
                    !x.IsUsed &&
                    x.Expiration > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (record == null)
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Invalid or expired OTP"
                };

            // تحقق من عدد المحاولات
            var maxAttempts = int.Parse(_configuration["Security:MaxOtpAttempts"]);

            if (record.Attempts >= maxAttempts)
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Too many attempts"
                };

            var hashedInput = Helper.HashOtp(dto.Code,
                _configuration["Security:OtpSecret"]);

            if (record.CodeHash != hashedInput)
            {
                record.Attempts++;
                await _context.SaveChangesAsync();

                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Invalid OTP"
                };
            }

            record.IsUsed = true;

            var sessionToken = Helper.GenerateSessionToken();

            await _context.SaveChangesAsync();

            return new AuthResult
            {
                IsSuccess = true,
                Message = "OTP verified",
                Token = sessionToken
            };
        }

        public async Task<AuthResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new AuthResult { IsSuccess = false, Message = "User not found" };

            // TODO: تحقق من sessionToken لو عايب أمان أكتر

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (!result.Succeeded)
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Reset failed",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };

            await _emailSender.SendEmailAsync(user.Email, "Password Reset Successful", "<p>Your password has been changed successfully.</p>");

            return new AuthResult { IsSuccess = true, Message = "Password reset successfully" };
        }

        private async Task CleanupOtpsAsync(string userId)
        {
            var expiredOtps = _context.PasswordResetOtps
                .Where(x => x.UserId == userId && (x.IsUsed || x.Expiration <= DateTime.UtcNow));

            _context.PasswordResetOtps.RemoveRange(expiredOtps);
            await _context.SaveChangesAsync();
        }

        public async Task<AuthResult> ResendOtpAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return new AuthResult { IsSuccess = false, Message = "User not found" };

            var lastOtp = await _context.PasswordResetOtps
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            // cooldown
            var cooldown = int.Parse(_configuration["Security:OtpCooldownSeconds"]);

            if (lastOtp != null &&
                lastOtp.CreatedAt > DateTime.UtcNow.AddSeconds(-cooldown))
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Please wait before requesting another OTP"
                };
            }

            // resend limit per hour
            var resendLimit = int.Parse(_configuration["Security:MaxResendPerHour"]);

            var resendCount = await _context.PasswordResetOtps
                .CountAsync(x =>
                    x.UserId == user.Id &&
                    x.CreatedAt > DateTime.UtcNow.AddHours(-1));

            if (resendCount >= resendLimit)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Too many requests, try again later"
                };
            }

            // delete old
            var oldOtps = _context.PasswordResetOtps
                .Where(x => x.UserId == user.Id);

            _context.PasswordResetOtps.RemoveRange(oldOtps);
            await _context.SaveChangesAsync();

            var code = Helper.GenerateOtp();
            var hash = Helper.HashOtp(code, _configuration["Security:OtpSecret"]);

            var otp = new PasswordResetOtp
            {
                UserId = user.Id,
                CodeHash = hash,
                Expiration = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Security:OtpExpiryMinutes"]))
            };

            _context.PasswordResetOtps.Add(otp);
            await _context.SaveChangesAsync();

            await _emailSender.SendEmailAsync(
                user.Email,
                "Your OTP Code",
                $"<h2>Your OTP is: {code}</h2>");

            return new AuthResult
            {
                IsSuccess = true,
                Message = "OTP resent successfully"
            };
        }

        public async Task<AuthResult> SignOutAllDevicesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "User not found"
                };

            // 🔐 تغيير SecurityStamp
            await _userManager.UpdateSecurityStampAsync(user);

            // 🧹 حذف كل Refresh Tokens
            var tokens = await _context.refreshTokens
                .Where(x => x.UserId == userId)
                .ToListAsync();

            _context.refreshTokens.RemoveRange(tokens);

            await _context.SaveChangesAsync();

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Signed out from all devices successfully"
            };
        }
    }
}
