using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Repository.Delivery
{
    public interface IDeliveryRepo
    {
        Task<List<Models.Delivery>> GetDeliveries(string sellerId);
        Task AddAsync(LoginOtp otp);
        Task<LoginOtp?> GetLatestValidOtpAsync(string userId);
        Task CleanupAsync(string userId);
        Task SaveChangesAsync();

        Task<Models.Delivery?> GetByAuctionIdAsync(int auctionId);
        Task AddTrackingAsync(OrderTracking tracking);
    }
}
