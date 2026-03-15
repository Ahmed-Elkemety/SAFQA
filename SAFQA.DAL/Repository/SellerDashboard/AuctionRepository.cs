using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Enums;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.SellerDashboard
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
    }
}