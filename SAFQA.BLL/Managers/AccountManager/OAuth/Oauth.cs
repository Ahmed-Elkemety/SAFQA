using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SAFQA.BLL.Help;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.DAL.Models;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SAFQA.DAL.Database;
using SAFQA.BLL.Dtos.AccountDto.Facebook;

namespace SAFQA.BLL.Managers.AccountManager.OAuth
{
    public class Oauth : IOAuth
    {
        #region Dependency Injection , UserManagement & SignInManager in Identity , IConfiguration To Access To App Settings
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly SAFQA_Context _context;

        public Oauth(UserManager<User> userManager
            , SignInManager<User> signInManager
            , IConfiguration configuration
            , SAFQA_Context context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }
        #endregion

        #region Google Login
        public async Task<AuthResult> GoogleLoginAsync(string idToken, string deviceId)
        {
            GoogleJsonWebSignature.Payload payload;

            try
            {
                var clientId = _configuration["Google:ClientId"];

                if (string.IsNullOrEmpty(clientId))
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Message = "Google ClientId is not configured"
                    };
                }

                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new List<string> { clientId }
                };

                payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Invalid Google token"
                };
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null)
            {
                user = new User
                {
                    Email = payload.Email,
                    UserName = payload.Email,
                    FullName = payload.Name,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Message = "Failed to create user",
                        Errors = createResult.Errors
                            .Select(e => e.Description)
                            .ToList()
                    };
                }

                var loginInfo = new UserLoginInfo(
                    loginProvider: "Google",
                    providerKey: payload.Subject,
                    displayName: "Google"
                );

                await _userManager.AddLoginAsync(user, loginInfo);
            }

            var tokens = await GenerateTokensAsync(user);

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Google login successful",
                Token = tokens
            };
        }
        #endregion

        #region Facebook Login
        public async Task<AuthResult> FacebookLoginAsync(string accessToken, string deviceId)
        {
            var fbUser = await VerifyFacebookToken(accessToken);

            if (fbUser == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Invalid Facebook token"
                };
            }

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u =>
                    u.FacebookId == fbUser.Id ||
                    (!string.IsNullOrEmpty(fbUser.Email) && u.Email == fbUser.Email));

            if (user == null)
            {
                user = new User
                {
                    FacebookId = fbUser.Id,
                    Email = fbUser.Email,
                    UserName = fbUser.Email ?? $"fb_{fbUser.Id}",
                    FullName = fbUser.Name,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Message = "Failed to create user",
                        Errors = createResult.Errors
                            .Select(e => e.Description)
                            .ToList()
                    };
                }

                await _userManager.AddLoginAsync(user,
                    new UserLoginInfo(
                        "Facebook",
                        fbUser.Id,
                        "Facebook"));
            }
            else if (string.IsNullOrEmpty(user.FacebookId))
            {
                user.FacebookId = fbUser.Id;
                await _userManager.UpdateAsync(user);
            }

            var tokens = await GenerateTokensAsync(user);

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Facebook login successful",
                Token = tokens
            };
        }

        private async Task<FacebookUserDto?> VerifyFacebookToken(string accessToken)
        {
            var appId = _configuration["Facebook:AppId"];
            var appSecret = _configuration["Facebook:AppSecret"];
            var url = $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appId}|{appSecret}";

            using var client = new HttpClient();
            var response = await client.GetStringAsync(url);
            var debugData = JsonSerializer.Deserialize<FacebookDebugResponse>(response);

            if (debugData?.Data?.IsValid != true ||
                debugData.Data.AppId != appId)
            {
                return null;
            }


            var userInfoUrl = $"https://graph.facebook.com/me?fields=id,name,email&access_token={accessToken}";
            var userInfoResponse = await client.GetStringAsync(userInfoUrl);
            return JsonSerializer.Deserialize<FacebookUserDto>(userInfoResponse);
        }

        private async Task<User> CreateUserAsync(FacebookUserDto fbUser)
        {
            var newUser = new User
            {
                FacebookId = fbUser.Id,
                Email = fbUser.Email, // ممكن يكون null
                UserName = fbUser.Email ?? fbUser.Id, // لو مفيش ايميل استخدم الـ id
                FullName = fbUser.Name
            };
            await _userManager.CreateAsync(newUser);
            return newUser;
        }
        #endregion

        #region GenerateTokens
        private async Task<string> GenerateTokensAsync(User user)
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

            await _context.SaveChangesAsync();

            return (new JwtSecurityTokenHandler().WriteToken(token));
        }
        #endregion
    }
}
