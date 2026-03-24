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

        public async Task<List<(string UserId, string Name, string Email, string CompanyName, int ParticipatedAuctions, decimal TotalPaid)>> GetTopCustomersAsync()
        {
            var result = await _context.AuctionUsers
                .Where(au =>
                    au.Auction.Status == AuctionStatus.Active ||
                    au.Auction.Status == AuctionStatus.Finished)

                .GroupBy(au => new
                {
                    au.User.Id,
                    au.User.FullName,
                    au.User.Email,
                    CompanyName = au.User.Seller != null ? au.User.Seller.StoreName : "Customer"
                })

                .Select(g => new ValueTuple<string, string, string, string, int, decimal>(
                    g.Key.Id,
                    g.Key.FullName,
                    g.Key.Email,
                    g.Key.CompanyName,

                    g.Select(x => x.AuctionId).Distinct().Count(),
                    
                    g.Sum(x =>
                        x.Auction.SecurityDeposit + 
                        (x.Auction.WinnerUserId == x.UserId ? x.Auction.FinalPrice : 0) 
                    )
                ))

                .OrderByDescending(x => x.Item5)
                .Take(5)

                .ToListAsync();

            return result;
        }

    }
}