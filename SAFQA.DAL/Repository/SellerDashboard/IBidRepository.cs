using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.SellerDashboard
{
    public interface IBidRepository
    {
        Task<int> GetSellerBids(int sellerId);
        Task<int> GetBidsByCategory(int sellerId, int categoryId);
        Task<List<(int AuctionId, string AuctionTitle, List<string> ProductNames, int TotalBids)>> GetAuctionsWithBidsRawBySellerAsync(int sellerId);
    }
}