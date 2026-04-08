using Google;
using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Enums;
using SAFQA.DAL.Database;
using SAFQA.DAL.RepoDtos.SellerApp.Bussiness_Account;
using SAFQA.DAL.RepoDtos.SellerApp.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Seller
{
    public class sellerRepo : IsellerRepo
    {
        private readonly SAFQA_Context _context;

        public sellerRepo(SAFQA_Context context)
        {
            _context = context;
        }

        public async Task<SellerBasic?> GetSellerBasicAsync(string userId)
        {
            return await _context.Sellers
                .Where(s => s.UserId == userId && !s.IsDeleted)
                .Select(s => new SellerBasic
                {
                    StoreName = s.StoreName,
                    StoreLogo = s.StoreLogo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<BusinessAccount?> GetBusinessAccountAsync(string userId)
        {
            return await _context.Sellers
                .Where(s => s.UserId == userId && !s.IsDeleted)
                .Select(s => new BusinessAccount
                {
                    StoreName = s.StoreName,
                    Email = s.User.Email,
                    PhoneNumber = s.PhoneNumber,

                    City = s.City != null ? s.City.Name : "",
                    Country = s.City != null && s.City.Country != null
                                ? s.City.Country.Name
                                : "",
                    Description = s.Description,

                    SellerRating = s.Rating,
                    Followers = s.Followers,

                    AuctionsCount = s.Auctions.Count(),

                    UpgradeType = s.upgradeType.ToString(),

                    StoreLogo = s.StoreLogo != null
                        ? Convert.ToBase64String(s.StoreLogo)
                        : null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetTotalSellersCount()
        {
            var activeSellers = await _context.Users
                .Where(u => u.Seller != null && u.Status == UserStatus.Active && !u.IsDeleted)
                .CountAsync();


            var blockedOrDeletedSellers = await _context.Users
                .Where(u => u.Seller != null && (u.Status == UserStatus.Blocked || u.IsDeleted))
                .CountAsync();


            return activeSellers + blockedOrDeletedSellers;
        }

        public async Task<int> GetVerifiedSellersCount()
        {
            return await _context.Sellers
                .Where(s => s.VerificationStatus == SellerVerificationStatus.Verified
                            && !s.IsDeleted
                            && s.User != null
                            && !s.User.IsDeleted
                            && s.User.Status == UserStatus.Active)
                .CountAsync();
        }

        public async Task<int> CountPendingSellers()
        {
            var pendingRegistered = await _context.Sellers
                .Where(s => s.VerificationStatus == SellerVerificationStatus.Pending
                            && !s.IsDeleted
                            && s.User != null
                            && !s.User.IsDeleted)
                .CountAsync();

            
            

            return pendingRegistered;
        }

        public IQueryable<Models.Seller> GetAll()
        {
            return _context.Sellers;
        }

        
        public Models.Seller GetById(int id)
        {
            return _context.Sellers.FirstOrDefault(s => s.Id == id);
        }
    }
}
