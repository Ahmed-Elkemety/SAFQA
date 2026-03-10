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
using SAFQA.DAL.Dtos.AccountDto.Facebook;
using SAFQA.BLL.Help;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.DAL.Models;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SAFQA.DAL.Database;

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
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

            if (payload == null)
                return new AuthResult { IsSuccess = false, Message = "Invalid Google token" };

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
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Message = "Failed to create user",
                        Errors = createResult.Errors.Select(e => e.Description).ToList()
                    };
            }

            var tokens = await GenerateTokensAsync(user, deviceId);

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Google login successful",
                UserId = user.Id,
                Token = tokens.Token,
                RefreshToken = tokens.RefreshToken
            };
        }
        #endregion

        #region Facebook Login
        public async Task<AuthResult> FacebookLoginAsync(string accessToken, string deviceId)
        {
            var fbUser = await VerifyFacebookToken(accessToken);
            if (fbUser == null)
                return new AuthResult { IsSuccess = false, Message = "Invalid Facebook token" };

            // جرب نجيب المستخدم عن طريق FacebookId أولاً
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FacebookId == fbUser.Id)
                       ?? await CreateUserAsync(fbUser);

            var tokens = await GenerateTokensAsync(user, deviceId);

            return new AuthResult
            {
                IsSuccess = true,
                UserId = user.Id,
                Token = tokens.Token,
                RefreshToken = tokens.RefreshToken
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

            if (debugData?.Data?.IsValid != true)
                return null;


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
        private async Task<(string Token, string RefreshToken)> GenerateTokensAsync(User user, string deviceId)
        {
            var claims = new List<Claim>
            {
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
