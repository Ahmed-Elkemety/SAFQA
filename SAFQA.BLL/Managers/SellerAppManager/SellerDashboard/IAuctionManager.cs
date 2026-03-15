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
        Task<int> GetTotalSellerAuctions(int sellerId);
        Task<int> GetActiveSellerAuctions(int sellerId);
    }
}