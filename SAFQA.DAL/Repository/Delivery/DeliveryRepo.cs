using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Repository.Delivery
{
    public class DeliveryRepo:IDeliveryRepo
    {
        private readonly SAFQA_Context _context;

        public DeliveryRepo(SAFQA_Context context)
        {
            _context = context;
        }
        public async Task<List<Models.Delivery>> GetDeliveries(string sellerId)
        {
            return await _context.Delivery
                .Where(d => d.Seller.UserId == sellerId)
                .Include(d => d.User)
                .Include(d => d.Auction)
                .ToListAsync();
        }

        public async Task AddAsync(LoginOtp otp)
        {
            await _context.LoginOtps.AddAsync(otp);
        }

        public async Task<LoginOtp?> GetLatestValidOtpAsync(string userId)
        {
            return await _context.LoginOtps
                .Where(x =>
                    x.UserId == userId &&
                    !x.IsUsed &&
                    x.Expiration > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task CleanupAsync(string userId)
        {
            var old = _context.LoginOtps
                .Where(x => x.UserId == userId);

            _context.LoginOtps.RemoveRange(old);

            await Task.CompletedTask;
        }

        public async Task<Models.Delivery?> GetByAuctionIdAsync(int auctionId)
        {
            return await _context.Delivery
                .FirstOrDefaultAsync(d => d.AuctionId == auctionId);
        }

        public async Task AddTrackingAsync(OrderTracking tracking)
        {
            await _context.OrderTracking.AddAsync(tracking);
        }

        public async Task SaveChangesAsync()
        {
            _context.deliveries.Remove(delivery);
            await _context.SaveChangesAsync();
        }
    }
}
