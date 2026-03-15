using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.SellerDashboard
{
    public class BidRepository : IBidRepository
    {
        private readonly SAFQA_Context _context;
        public BidRepository(SAFQA_Context context) 
        {
            _context = context;
        }

        public async Task<int> GetBidsByCategory(int sellerId, int categoryId)
        {
            var totalBids = await _context.Bids
                                 .Where(b => b.Auction.SellerId == sellerId && b.Auction.items.Any(i => i.CategoryId == categoryId))
                                 .CountAsync();
            return totalBids;
        }

        public Task<int> GetSellerBids(int sellerId)
        {
            return _context.Bids
                            .Where(b => b.Auction.SellerId == sellerId)
                            .CountAsync();
        }

        public async Task<List<(int AuctionId, string AuctionTitle, List<string> ProductNames, int TotalBids)>> GetAuctionsWithBidsRawBySellerAsync(int sellerId)
        {
            return await _context.Auctions
                        .Where(a => a.SellerId == sellerId && !a.IsDeleted)
                        .Select(a => new ValueTuple<int, string, List<string>, int>(
                            a.Id,
                            a.Title,
                            a.items.Select(i => i.title).ToList(),
                            a.Bids.Count()
                        ))
                        .ToListAsync();
        }
    }
}