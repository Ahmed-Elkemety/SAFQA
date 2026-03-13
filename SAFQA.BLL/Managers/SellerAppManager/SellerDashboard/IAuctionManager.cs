using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.SellerDashboard
{
    public interface IAuctionManager
    {
        Task<int> GetTotalAuctionsAsync(int sellerId);
        IQueryable<Auction> GetActiveAuctions(int sellerId);
        IQueryable<Auction> GetAllAuctions(int sellerId);
    }
}
