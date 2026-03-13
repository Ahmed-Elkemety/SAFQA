using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.SellerDashboard
{
    public interface IAuctionRepository
    {
        IQueryable<Auction> GetSellerAuctions(int sellerId);
        IQueryable<Auction> GetActiveSellerAuctions(int sellerId);
        Task<int> CountAuctionsBySellerAsync(int sellerId);
    }
}

/*   
    _context.Auctions.where(a => a.SellerId == Seller.SellerId) 
*/ 
