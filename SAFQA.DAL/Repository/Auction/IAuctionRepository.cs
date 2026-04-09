using SAFQA.DAL.Models;
using SAFQA.DAL.RepoDtos.UserApp.Home.TrendingAuction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Auction
{
    public interface IAuctionRepository
    {
        IQueryable<Models.Auction> GetAll();
        Models.Auction GetById(int Id);
        Task<int> GetActiveSellerAuctions(int sellerId);
        Task<int> CountAuctionsBySeller(int sellerId);
        Task<List<(User User, Models.Seller Seller, Models.Auction AuctionDetails)>> GetSellerWinnersRawAsync(int sellerId);
        Task<List<(string UserId, string Name, string Email, string CompanyName, int ParticipatedAuctions, decimal TotalPaid)>> GetTopCustomersAsync();
        Task<int> GetTotalAuctionsCount();
        Task<int> GetActiveAuctionsCount();
        Task<int> GetExpiredAuctionsCount();
        Task<int> GetUpcomingAuctionsCount();

        Task<List<TrendingAuction>> GetTrendingAuctionsAsync();
        IQueryable<Models.Auction> GetSellerAuctions(string userId);
        void Add(Models.Auction auction);
        void Update(Models.Auction auction);
        void Delete(Models.Auction auction);
    }
}