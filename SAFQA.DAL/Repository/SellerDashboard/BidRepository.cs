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

        public IQueryable<Bid> GetBidsByCategory(int sellerId, int categoryId)
        {
            return _context.Bids
                .Where(b => b.Auction.SellerId == sellerId && b.Auction.items
                .Any(i => i.CategoryId == categoryId));
        }

        public IQueryable<Bid> GetSellerBids(int sellerId)
        {
            return _context.Bids
                .Where(b => b.Auction.SellerId == sellerId);
        }
    }
}