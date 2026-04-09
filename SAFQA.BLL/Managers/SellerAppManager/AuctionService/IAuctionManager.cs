using SAFQA.BLL.Dtos.SellerAppDto.AuctionDto;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.BLL.Enums;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SAFQA.BLL.Help.Helper;

namespace SAFQA.BLL.Managers.SellerAppManager.AuctionService
{
    public interface IAuctionManager
    {
        Task<int> GetTotalSellerAuctions(int sellerId);
        Task<int> GetActiveSellerAuctions(int sellerId);
        Task<List<TopCustomerDto>> GetTopCustomers();
        Task<int> GetTotalAuctions();
        Task<int> GetActiveAuctionsCount();
        Task<int> GetExpiredAuctionsCount();
        Task<int> GetUpcomingAuctionsCount();
        Task<List<AuctionProfitDto>> GetTopProfitableAuctions(int sellerId, int categoryId);
        Task<IEnumerable<CategoryPercentageDto>> GetCategoryPercentageBySeller(int sellerId);
        Task<IEnumerable<AuctionBidsDto>> GetSellerAuctionsBids(int sellerId);
        IEnumerable<MonthlyEarningDto> GetMonthlyEarningsByCategory(int sellerId, int categoryId);
        IEnumerable<PopularProductsDto> GetMostPopularProductsBySeller(int sellerId, int topCount = 10);

        Task<IEnumerable<TopWinnerDto>> GetWinnersBySeller(int sellerId);
            Task<PagedResult<SellerActionHistoryDto>> GetHistory(
                string userId,
                AuctionStatus? status,
                int page,
                int pageSize);
    }
}