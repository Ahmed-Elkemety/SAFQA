using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Bids
{
    public class BidRepository : IBidRepository
    {
        private readonly SAFQA_Context _context;
        public BidRepository(SAFQA_Context context) 
        {
            _context = context;
        }

        public Task<int> GetBidsByCategory(string userId, int categoryId)
        {
            return _context.Bids
                .Where(b =>
                    b.Auction.Seller.UserId == userId &&
                    b.Auction.CategoryId == categoryId
                )
                .CountAsync();
        }

        public Task<int> GetSellerBids(string userId)
        {
            return _context.Bids
                .Where(b => b.Auction.Seller.UserId == userId)
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
        public IQueryable<Bid> GetAll()
        {
            return _context.Bids;
        }

        public Bid GetById(int Id)
        {
            return _context.Bids.FirstOrDefault(s => s.Id == Id);
        }

    }
}