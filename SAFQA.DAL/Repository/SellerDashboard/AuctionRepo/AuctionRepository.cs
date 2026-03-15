using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Enums;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.SellerDashboard.AuctionRepo
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly SAFQA_Context _context;

        public AuctionRepository(SAFQA_Context context)
        {
            _context = context;
        }

        public Task<int> CountAuctionsBySeller(int sellerId)
        {
            return _context.Auctions
                    .Where(a => a.SellerId == sellerId)
                    .CountAsync();
        }

        public Task<int> GetActiveSellerAuctions(int sellerId)
        {
            return _context.Auctions
                            .Where(a => a.SellerId == sellerId && a.Status == AuctionStatus.Active)
                            .CountAsync();
        }


        public async Task<List<(User User, Models.Seller seller , Models.Auction AuctionDetails)>> GetSellerWinnersRawAsync(int sellerId)
        {
            var query = await _context.Auctions
        .Where(a => a.SellerId == sellerId &&
                    !string.IsNullOrEmpty(a.WinnerUserId) &&
                    a.Status == AuctionStatus.Finished)
        .Join(
            _context.Users,
            a => a.WinnerUserId,
            u => u.Id,
            (a, u) => new { Auction = a, User = u }
        )
        .Where(x => x.Auction != null && x.User != null && x.Auction.Seller != null)
        .ToListAsync();

            return query.Select(x => (x.User, x.Auction.Seller, AuctionDetails: x.Auction)).ToList();
        }
    }
}