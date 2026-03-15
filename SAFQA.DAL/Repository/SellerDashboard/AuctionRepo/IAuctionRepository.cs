using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.SellerDashboard.AuctionRepo
{
    public interface IAuctionRepository
    {
        Task<int> GetActiveSellerAuctions(int sellerId);
        Task<int> CountAuctionsBySeller(int sellerId);
        Task<List<(User User, Models.Seller seller, Models.Auction AuctionDetails)>> GetSellerWinnersRawAsync(int sellerId);
    }
}
