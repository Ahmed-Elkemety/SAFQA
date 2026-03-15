using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Dtos.AccountDto.Seller;
using SAFQA.BLL.Dtos.SellerAppDto.HomeDto;
using SAFQA.BLL.Enums;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.DAL.Database;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;

namespace SAFQA.BLL.Managers.SellerAppManager
{
    public class sellerManager : IsellerManager
    {
        private readonly SAFQA_Context _context;
        private readonly UserManager<User> _userManager;

        public sellerManager(SAFQA_Context context , UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task CreateSellerAsync(string userId, CreateSellerDto dto)
        {

            var existingSeller = await _context.Sellers
            .FirstOrDefaultAsync(s => s.UserId == userId);

            if (existingSeller != null)
            {
                throw new InvalidOperationException("Seller already exists for this user.");
            }

            var seller = new Seller
            {
                UserId = userId,
                CityId = dto.CityId,
                StoreName = dto.StoreName,
                PhoneNumber = dto.PhoneNumber,
                BussinessType = dto.BusinessType,
                Description = dto.Description,
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

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var isInRole = await _userManager.IsInRoleAsync(user, "Seller");
                if (!isInRole)
                {
                    await _userManager.AddToRoleAsync(user, "Seller");
                }
            }
        }

        private async Task<byte[]> ConvertFileToBytes(IFormFile file)
        {
            using var memoryStream = new MemoryStream();

            await file.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

        public async Task<AuthResult> UploadPersonalDocsAsync(int sellerId, PersonalSellerDto dto)
        {
            try
            {
                var seller = await _context.Sellers.FindAsync(sellerId);
                if (seller == null)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Message = "Seller not found"
                    };
                }

                var personalSeller = new PersonalSeller
                {
                    SellerId = sellerId,
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

        public async Task<AuthResult> UploadBusinessDocsAsync(int sellerId, BusinessSellerDto dto)
        {
            try
            {
                var seller = await _context.Sellers
                    .Include(s => s.City)
                    .ThenInclude(c => c.Country)
                    .FirstOrDefaultAsync(s => s.Id == sellerId);

                if (seller == null)
                {
                    return new AuthResult
                    {
                        IsSuccess = false,
                        Message = "Seller not found"
                    };
                }

                // Instapay يضاف فقط لو الدولة مصر
                int? InstapayNumber = null;
                if (seller.City.CountryId == 1) // 1 = Egypt
                {
                    InstapayNumber = dto.instaPayNumber;
                }

                var businessSeller = new BusinessSeller
                {
                    SellerId = sellerId,

                    // Legal Documents
                    CommercialRegister = await ConvertFileToBytes(dto.CommercialRegister),
                    TaxId = await ConvertFileToBytes(dto.TaxId),
                    OwnerNationalIdFront = await ConvertFileToBytes(dto.OwnerNationalIdFront),
                    OwnerNationalIdBack = await ConvertFileToBytes(dto.OwnerNationalIdBack),

                    // Financial Details
                    BankName = dto.BankName,
                    AccountName = dto.AccountName,
                    IBAN = dto.IBAN,
                    LocalAccountNumber = dto.LocalAccountNumber,

                    // Egypt Only
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
    }
}
