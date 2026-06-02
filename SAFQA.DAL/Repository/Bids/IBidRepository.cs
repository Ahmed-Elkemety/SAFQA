using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Bids
{
    public interface IBidRepository
    {
        IQueryable<Bid> GetAll();
        Bid GetById(int Id);
        Task<int> GetSellerBids(string userId);
        Task<int> GetBidsByCategory(string userId, int categoryId);
        Task<List<(int AuctionId, string AuctionTitle, List<string> ProductNames, int TotalBids)>> GetAuctionsWithBidsRawBySellerAsync(int sellerId);
    }
}