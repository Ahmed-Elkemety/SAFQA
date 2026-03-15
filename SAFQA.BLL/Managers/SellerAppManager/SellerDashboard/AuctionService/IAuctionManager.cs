using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.SellerDashboard.AuctionService
{
    public interface IAuctionManager
    {
        Task<int> GetTotalSellerAuctions(int sellerId);
        Task<int> GetActiveSellerAuctions(int sellerId);
        Task<List<SellerWinnerDto>> GetSellerWinnersAsync(int sellerId);
    }
}