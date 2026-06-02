using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.BidService
{
    public interface IBidManager
    {
        Task<int> GetBidsByCategory(string userId, int categoryId);

        Task<int> GetSellerBids(string userId);
    }
}
