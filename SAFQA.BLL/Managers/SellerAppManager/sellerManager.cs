using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SAFQA.BLL.Dtos.AccountDto.Seller;
using SAFQA.BLL.Dtos.SellerAppDto.HomeDto;
using SAFQA.BLL.Enums;
using SAFQA.BLL.Help;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.DAL.Database;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Seller;

namespace SAFQA.BLL.Managers.SellerAppManager
{
    public class sellerManager : IsellerManager
    {
        private readonly SAFQA_Context _context;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IsellerRepo _IsellerRepo;

        public sellerManager(SAFQA_Context context , IConfiguration configuration,  UserManager<User> userManager, IsellerRepo isellerRepo)
        {
            _configuration = configuration;
            _context = context;
            _userManager = userManager;
            _IsellerRepo = isellerRepo;
        }

        private async Task<(string Token, string RefreshToken)> GenerateTokensAsync(User user, string deviceId)
        {
            // 1️⃣ إعداد الـ Claims الأساسية
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("SecurityStamp", user.SecurityStamp),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


            //2️⃣ إضافة الـ Roles كـ Claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            // 3️⃣ إنشاء الـ JWT Token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            // 4️⃣ إنشاء Refresh Token آمن
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshTokenHash = refreshToken.Hash(); // تأكد إنك عندك extension method Hash()

            // 5️⃣ تخزين Refresh Token في الـ Database
            _context.refreshTokens.Add(new RefreshToken
            {
                TokenHash = refreshTokenHash,
                UserId = user.Id,
                DeviceId = deviceId,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });

            await _context.SaveChangesAsync();

            // 6️⃣ إعادة الـ Token و الـ Refresh Token
            return (new JwtSecurityTokenHandler().WriteToken(token), refreshToken);
        }

        public async Task<AuthResult> CreateSellerAsync(string userId, CreateSellerDto dto , string deviceId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "User not found"
                };
            }
            var existingSeller = await _context.Sellers
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (existingSeller != null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Seller already exists",
                    Errors = new List<string> { "Seller already exists for this user." }
                };
            }

            byte[]? logoBytes = null;

            if (dto.Logo != null)
            {
                var allowedTypes = new[] { "image/png", "image/jpeg", "image/jpg" };

                if (!allowedTypes.Contains(dto.Logo.ContentType.ToLower()))
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Message = "Logo must be PNG or JPG",
                        Errors = new List<string> { "Invalid file type for Logo" }
                    };
                }

                logoBytes = await ConvertFileToBytes(dto.Logo);
            }

            var seller = new Seller
            {
                UserId = userId,
                CityId = dto.CityId,
                StoreName = dto.StoreName,
                PhoneNumber = dto.PhoneNumber,
                BussinessType = dto.BusinessType,
                Description = dto.Description,
                StoreLogo = logoBytes,
                VerificationStatus = SellerVerificationStatus.Pending,
                StoreStatus = StoreStatus.Active,
                SellerAt = DateTime.UtcNow,
                Rating = 0.0f,
                Followers = 0,
                AuctionCount = 0,
                upgradeType = UpgradeType.None,
                IsDeleted = false
            };

            _context.Sellers.Add(seller);
            await _context.SaveChangesAsync();

            // إضافة Role
            var isInRole = await _userManager.IsInRoleAsync(user, "SELLER");
            if (!isInRole)
            {
                await _userManager.AddToRoleAsync(user, "SELLER");
            }

            // توليد Token جديد
            var token = await GenerateTokensAsync(user , deviceId);

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Seller created successfully",
                UserId = user.Id,
                Token = token.Token,
                RefreshToken = token.RefreshToken
            };
        }

        private async Task<byte[]> ConvertFileToBytes(IFormFile file)
        {
            using var memoryStream = new MemoryStream();

            await file.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

        public async Task<AuthResult> UploadPersonalDocsAsync(string userId, PersonalSellerDto dto)
        {
            try
            {
                var seller = await _context.Sellers
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (seller == null)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Message = "Seller not found"
                    };
                }

                // منع التكرار
                var existing = await _context.personalSellers
                    .FirstOrDefaultAsync(p => p.SellerId == seller.Id);

                if (existing != null)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Message = "Documents already uploaded"
                    };
                }

                // Validation
                if (dto.NationalIdFront == null || dto.NationalIdBack == null || dto.SelfieWithId == null)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Message = "All files are required"
                    };
                }

                var allowedTypes = new[] { "image/png", "image/jpeg", "image/jpg" };

                if (!allowedTypes.Contains(dto.NationalIdFront.ContentType.ToLower()) ||
                    !allowedTypes.Contains(dto.NationalIdBack.ContentType.ToLower()) ||
                    !allowedTypes.Contains(dto.SelfieWithId.ContentType.ToLower()))
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Message = "Files must be PNG or JPG"
                    };
                }

                var personalSeller = new PersonalSeller
                {
                    SellerId = seller.Id,
                    NationalIdFront = await ConvertFileToBytes(dto.NationalIdFront),
                    NationalIdBack = await ConvertFileToBytes(dto.NationalIdBack),
                    SelfieWithId = await ConvertFileToBytes(dto.SelfieWithId)
                };

                _context.personalSellers.Add(personalSeller);
                await _context.SaveChangesAsync();

                return new AuthResult
                {
                    IsSuccess = true,
                    Message = "Personal documents uploaded successfully"
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Failed to upload personal documents",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<AuthResult> UploadBusinessDocsAsync(string userId, BusinessSellerDto dto)
        {
            try
            {
                var seller = await _context.Sellers
                    .Include(s => s.City)
                    .ThenInclude(c => c.Country)
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (seller == null)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Message = "Seller not found"
                    };
                }

                var existing = await _context.businessSellers
                    .FirstOrDefaultAsync(b => b.SellerId == seller.Id);

                if (existing != null)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Message = "Documents already uploaded"
                    };
                }

                if (dto.CommercialRegister == null || dto.TaxId == null ||
                     dto.OwnerNationalIdFront == null || dto.OwnerNationalIdBack == null)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Message = "All required files must be uploaded"
                    };
                }

                var allowedTypes = new[] { "image/png", "image/jpeg", "image/jpg" };

                if (!allowedTypes.Contains(dto.CommercialRegister.ContentType.ToLower()) ||
                    !allowedTypes.Contains(dto.TaxId.ContentType.ToLower()) ||
                    !allowedTypes.Contains(dto.OwnerNationalIdFront.ContentType.ToLower()) ||
                    !allowedTypes.Contains(dto.OwnerNationalIdBack.ContentType.ToLower()))
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Message = "Files must be PNG or JPG"
                    };
                }

                int? InstapayNumber = null;
                if (seller.City.CountryId == 1)
                {
                    InstapayNumber = dto.instaPayNumber;
                }

                var businessSeller = new BusinessSeller
                {
                    SellerId = seller.Id,

                    CommercialRegister = await ConvertFileToBytes(dto.CommercialRegister),
                    TaxId = await ConvertFileToBytes(dto.TaxId),
                    OwnerNationalIdFront = await ConvertFileToBytes(dto.OwnerNationalIdFront),
                    OwnerNationalIdBack = await ConvertFileToBytes(dto.OwnerNationalIdBack),

                    BankName = dto.BankName,
                    AccountName = dto.AccountName,
                    IBAN = dto.IBAN,
                    LocalAccountNumber = dto.LocalAccountNumber,

                    instaPayNumber = InstapayNumber,
                };

                _context.businessSellers.Add(businessSeller);
                await _context.SaveChangesAsync();

                return new AuthResult
                {
                    IsSuccess = true,
                    Message = "Business documents uploaded successfully"
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "Failed to upload business documents",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        public async Task<SellerBasicDto?> GetMySellerHomeAsync(string userId)
        {
            return await _context.Sellers
                .Where(s => s.UserId == userId && !s.IsDeleted)
                .Select(s => new SellerBasicDto
                {
                    StoreName = s.StoreName,
                    StoreLogo = s.StoreLogo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetTotalSellersCount()
        {
            return await _IsellerRepo.GetTotalSellersCount();
        }

        public async Task<int> GetVerifiedSellersCount()
        {
            return await _IsellerRepo.GetVerifiedSellersCount();
        }

        public async Task<int> GetPendingSellersCount()
        {
            return await _IsellerRepo.CountPendingSellers();
        }
    }
}
