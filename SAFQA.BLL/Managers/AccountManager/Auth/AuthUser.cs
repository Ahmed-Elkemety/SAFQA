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
using Newtonsoft.Json.Linq;
using static SAFQA.BLL.Dtos.AccountDto.User.LocationDto;
using SAFQA.DAL.Repository.Location;



namespace SAFQA.BLL.Managers.AccountManager.Auth
{
    public class AuthUser : IAuthUser
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly SAFQA_Context _context;
        private readonly ILocationRepo _locationRepo;

        public AuthUser(UserManager<User> userManager
            , SignInManager<User> signInManager
            , IConfiguration configuration
            , IEmailSender emailSender
            , SAFQA_Context context,
            ILocationRepo locationRepo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _context = context;
            _locationRepo = locationRepo;
        }

        public async Task<AuthResult> RegisterAsync(RegisterDto dto , string deviceId)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                var isSeller = await _context.Sellers
                    .AnyAsync(s => s.UserId == existingUser.Id);

                if (isSeller)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Errors = new() { "Email already registered as Seller" }
                    };
                }

                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new() { "Email already Used" }
                };
            }

            var pendingUser = await _context.PendingUserRegistrations
                .FirstOrDefaultAsync(x => x.Email == dto.Email && !x.IsUsed);

            if (pendingUser != null)
            {
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

            var passwordValidator = new PasswordValidator<User>();

            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            var validationResult = await passwordValidator.ValidateAsync(_userManager, user, dto.Password);

            if (!validationResult.Succeeded)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = validationResult.Errors.Select(e => e.Description).ToList()
                };
            }

            var city = await _context.cities
                .FirstOrDefaultAsync(c => c.Id == dto.cityId);

            if (city == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new() { "Invalid City" }
                };
            }
            var otp = Helper.GenerateOtp();

            string otpSecret = _configuration["Security:OtpSecret"];
            int otpExpiryMinutes = 5; // default
            var otpExpiryConfig = _configuration["Security:OtpExpiryMinutes"];
            if (!string.IsNullOrEmpty(otpExpiryConfig))
                otpExpiryMinutes = int.Parse(otpExpiryConfig);

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

            await _emailSender.SendOtpEmailAsync(dto.Email, otp);

            return new AuthResult
            {
                IsSuccess = true,
                Message = "OTP sent to your email"
            };
        }

        public async Task<AuthResult> ConfirmEmailAsync(ConfirmEmailDto dto)
        {
            var pending = await _context.PendingUserRegistrations
                .Where(x => x.Email == dto.Email && !x.IsUsed && x.OtpExpiration > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (pending == null)
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new() { "Invalid or expired OTP" }
                };

            var otpHash = Helper.HashOtp(dto.Otp, _configuration["Security:OtpSecret"]);
            if (otpHash != pending.OtpHash)
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new() { "Invalid OTP" }
                };

            var user = new User
            {
                FullName = pending.FullName,
                Email = pending.Email,
                UserName = pending.Email,
                PhoneNumber = pending.PhoneNumber,
                Gender = pending.Gender,
                BirthDate = (DateOnly)pending.BirthDate,
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



            await _userManager.AddToRoleAsync(user, "USER");

            pending.IsUsed = true;
            await _context.SaveChangesAsync();

            var walletExists = await _context.Wallets
                .AnyAsync(w => w.UserId == user.Id);

            if (!walletExists)
            {
                var wallet = new Wallet
                {
                    UserId = user.Id,
                    Balance = 0,
                    FrozenBalance = 0,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Wallets.AddAsync(wallet);
                await _context.SaveChangesAsync();
            }


            return new AuthResult
            {
                IsSuccess = true,
                Message = "Email confirmed and account created successfully"
            };
        }

        public async Task<AuthResult> LoginAsync(LoginDto dto, string deviceId , string role)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                var pendingUser = await _context.PendingUserRegistrations
                    .FirstOrDefaultAsync(p => p.Email == dto.Email);

                if (pendingUser != null)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Email registered but OTP not verified. Please verify your email." }
                    };
                }

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


            var token = await GenerateTokensAsync(user, deviceId);

            if(role == "seller")
            {
                var isSeller = await _context.Sellers
                    .AnyAsync(s => s.UserId == user.Id);
                if (!isSeller)
                {
                    return new AuthResult
                    {
                        IsSuccess = true,
                        UserId = user.Id,
                        Token = token.Token,
                        RefreshToken = token.RefreshToken,
                        Message = "Login successful As User"
                    };
                }
                else
                {
                    return new AuthResult
                    {
                        IsSuccess = true,
                        UserId = user.Id,
                        Token = token.Token,
                        RefreshToken = token.RefreshToken,
                        Message = "Login successful As Seller"

                    };
                }
            }
            else
            {
                return new AuthResult
                {
                    IsSuccess = true,
                    UserId = user.Id,
                    Token = token.Token,
                    RefreshToken = token.RefreshToken,
                    Message = "Login successful As User"
                };
            }
        }

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

        private async Task<(string Token, string RefreshToken)> GenerateTokensAsync(User user, string deviceId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("SecurityStamp", user.SecurityStamp),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


           var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(5),
                signingCredentials: creds
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

        public async Task<AuthResult> ResendRegistrationOtpAsync(string email)
        {
            var pendingUser = await _context.PendingUserRegistrations
                .FirstOrDefaultAsync(u => u.Email == email);

            if (pendingUser == null)
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Pending user not found"
                };

            var cooldown = int.Parse(_configuration["Security:OtpCooldownSeconds"]);

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

            var code = Helper.GenerateOtp();
            var hash = Helper.HashOtp(code, _configuration["Security:OtpSecret"]);

            pendingUser.OtpHash = hash;
            pendingUser.OtpExpiration = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Security:OtpExpiryMinutes"]));
            pendingUser.IsUsed = false;
            pendingUser.LastOtpSentAt = DateTime.UtcNow; 

            _context.PendingUserRegistrations.Update(pendingUser);
            await _context.SaveChangesAsync();

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

            await CleanupOtpsAsync(user.Id);

            var code = Helper.GenerateOtp();
            var hashed = Helper.HashOtp(code, _configuration["Security:OtpSecret"]);

            _context.PasswordResetOtps.Add(new PasswordResetOtp
            {
                UserId = user.Id,
                CodeHash = hashed,
                Expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Security:OtpExpiryMinutes"])),
                IsUsed = false
            });
            await _context.SaveChangesAsync();

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

        public async Task<AuthResult> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            if(dto.CurrentPassword == dto.NewPassword)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Current Password Can not be Match With New Password"
                };
            }

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Passwords do not match"
                };
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "User not found"
                };
            }
            var isCorrect = await _userManager.CheckPasswordAsync(user, dto.CurrentPassword);

            if (!isCorrect)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Current password is incorrect"
                };
            }

                var passwordValidator = new PasswordValidator<User>();


                var validationResult = await passwordValidator.ValidateAsync(_userManager, user, dto.NewPassword);

                if (!validationResult.Succeeded)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Errors = validationResult.Errors.Select(e => e.Description).ToList()
                    };
                }


                var result = await _userManager.ChangePasswordAsync(
                user,
                dto.CurrentPassword,
                dto.NewPassword
            );

            if (!result.Succeeded)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Change password failed",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Password changed successfully"
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

            await _userManager.UpdateSecurityStampAsync(user);

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

        public async Task<List<CountryDto>> GetCountriesAsync()
        {
            var data = await _locationRepo.GetCountriesAsync();

            return data.Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
        }

        public async Task<List<CityDto>> GetCitiesByCountryIdAsync(int countryId)
        {
            var data = await _locationRepo.GetCitiesByCountryIdAsync(countryId);

            return data.Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
        }
    }
}
