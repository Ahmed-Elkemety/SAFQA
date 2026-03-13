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
        IQueryable<Bid> GetBidsByCategory(int sellerId, int categoryId);

        IQueryable<Bid> GetSellerBids(int sellerId);
    }
}
