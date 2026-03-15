using SAFQA.BLL.Managers.Dtos;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.SellerDashboard
{
    public interface IBidManager
    {
        Task<int> GetBidsByCategory(int sellerId, int categoryId);

        Task<int> GetSellerBids(int sellerId);

        Task<List<AuctionBidsDto>> GetTop4AuctionsBySeller(int sellerId);
    }
}
