using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SAFQA.BLL.Dtos.AccountDto;
using SAFQA.BLL.Enums;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace SAFQA.BLL.Managers.AccountManager
{
    public class AuthUser : IAuthUser
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly SAFQA_Context _context;

        public AuthUser(UserManager<User> userManager
            , SignInManager<User> signInManager 
            , IConfiguration configuration 
            , SAFQA_Context context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }
        public async Task<AuthResult> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Email already in use" }
                };
            }
            var user = new User
            {
                FullName = dto.FullName,
                Gender = dto.Gender,
                BirthDate = dto.BirthDate,
                Role = dto.Role,
                Status = dto.Status,
                Language = dto.Language,
                Email = dto.Email,
                UserName = dto.Email
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
            await _userManager.AddToRoleAsync(user, dto.Role.ToString().ToUpper());

            var token = await  GenerateTokensAsync(user);

            return new AuthResult
            {
                IsSuccess = true,
                UserId = user.Id,
                Token = token.Token,
                RefreshToken = token.RefreshToken
            };
        }
        public async Task<AuthResult> LoginAsync(LoginDto dto)
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
            var token = await GenerateTokensAsync(user);
            return new AuthResult
            {
                IsSuccess = true,
                UserId = user.Id,
                Token = token.Token,
                RefreshToken = token.RefreshToken
            };
        }

        public async Task<AuthResult> RefreshTokenAsync(string Token)
        {
            var userToken = await _context.refreshTokens
           .Include(r => r.User)
           .FirstOrDefaultAsync(r => r.Token == Token);

            if (userToken == null || userToken.ExpiryDate < DateTime.UtcNow)
            return new AuthResult { IsSuccess = false, Message = "Invalid or expired refresh token" };

        var tokens = await GenerateTokensAsync(userToken.User);

        userToken.Token = tokens.RefreshToken;
        userToken.ExpiryDate = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new AuthResult
        {
            IsSuccess = true,
            UserId = userToken.User.Id,
            Token = tokens.Token,
            RefreshToken = tokens.RefreshToken
        };
        }

       private async Task<(string Token, string RefreshToken)> GenerateTokensAsync(User user)
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

            var refreshToken = Guid.NewGuid().ToString();


            _context.refreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            });
            await _context.SaveChangesAsync();

            return (new JwtSecurityTokenHandler().WriteToken(token), refreshToken);
        }

    }
}
