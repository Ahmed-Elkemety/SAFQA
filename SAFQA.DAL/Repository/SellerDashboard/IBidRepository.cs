using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.SellerDashboard
{
    public interface IBidRepository
    {
        IQueryable<Bid> GetSellerBids(int sellerId);
        IQueryable<Bid> GetBidsByCategory(int sellerId, int categoryId);
    }
}
