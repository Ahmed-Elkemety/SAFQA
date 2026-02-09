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
        public async Task<AuthResult> RegisterAsync(RegisterDto dto, string deviceId)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new() { "Email already in use" }
                };

            var otp = RandomNumberGenerator // Replace By Helper
                .GetInt32(100000, 999999)
                .ToString();

            var user = new User
            {
                FullName = dto.FullName,
                Gender = dto.Gender,
                BirthDate = dto.BirthDate,
                Status = UserStatus.Active,
                Email = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                CityId = dto.cityId,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            await _userManager.AddToRoleAsync(user, "USER");

            var otpEntity = new PasswordResetOtp
            {
                UserId = user.Id, 
                CodeHash = BCrypt.Net.BCrypt.HashPassword(otp),
                Expiration = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            _context.PasswordResetOtps.Add(otpEntity);
            await _context.SaveChangesAsync();

            await _emailSender.SendOtpEmailAsync(user.Email, otp);

            return new AuthResult
            {
                IsSuccess = true,
                Message = "OTP sent to your email"
            };
        }
        #endregion

        #region Confirmation Email By Using OTP
        public async Task<AuthResult> ConfirmEmailAsync(ConfirmEmailDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new() { "User not found" }
                };

            if (user.EmailConfirmed)
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new() { "Email already confirmed" }
                };

            var otpEntity = await _context.PasswordResetOtps
                .Where(x =>
                    x.UserId == user.Id &&
                    !x.IsUsed &&
                    x.Expiration > DateTime.UtcNow
                )
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (otpEntity == null)
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new() { "Invalid or expired OTP" }
                };

            var isValidOtp = BCrypt.Net.BCrypt.Verify(dto.Otp, otpEntity.CodeHash);
            if (!isValidOtp)
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new() { "Invalid or expired OTP" }
                };

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            otpEntity.IsUsed = true;
            await _context.SaveChangesAsync();

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Email confirmed successfully"
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
    }
}
