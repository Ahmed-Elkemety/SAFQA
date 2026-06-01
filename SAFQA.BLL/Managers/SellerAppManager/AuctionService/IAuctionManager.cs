using SAFQA.BLL.Dtos.SellerAppDto.AuctionDto;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.DAL.Enums;
using SAFQA.BLL.Managers.AccountManager.Auth;
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
        Task<int> GetTotalSellerAuctions(string userId);
        Task<int> GetActiveSellerAuctions(string userId);
        Task<List<TopCustomerDto>> GetTopCustomers(string SellerUserId);
        Task<int> GetTotalAuctions();
        Task<int> GetActiveAuctionsCount();
        Task<int> GetExpiredAuctionsCount();
        Task<int> GetUpcomingAuctionsCount();
        Task<List<AuctionProfitDto>> GetTopProfitableAuctions(string userId, int categoryId);
        Task<IEnumerable<CategoryPercentageDto>> GetCategoryPercentageBySeller(string sellerUserId);
        Task<IEnumerable<AuctionBidsDto>> GetSellerAuctionsBids(string SellerUserId);
        IEnumerable<MonthlyEarningDto> GetMonthlyEarningsByCategory(string sellerUserId, int categoryId);
        IEnumerable<PopularProductsDto> GetMostPopularProductsBySeller(string sellerUserId, int topCount = 5);
        PagedResult<ExpiredAuctionsDto> GetExpiredAuctions(int page = 1, int pageSize = 10);
        void DeleteAuctionPermanently(int id);
        PagedResult<ActiveAuctionDto> GetActiveAuctions(int page, int pageSize);
        void ForceExpireAuction(int auctionId);
        PagedResult<RejectedDeletedAuctionDto> GetRejectedDeletedAuctions(int page = 1, int pageSize = 10);
        void DeleteAuctionPermanentlyy(int id);
        Task<IEnumerable<TopWinnerDto>> GetWinnersBySeller(string SellerUserId);
        Task<PagedResult<SellerActionHistoryDto>> GetHistory(
                string userId,
                AuctionStatus? status,
                int page,
                int pageSize);

        Task<AuthResult> CreateAuction(CreateAuctionDto dto, string userId);
        Task<AuthResult> EditAuction(int id, EditAuctionDto dto, string userId);
        Task<AuthResult> DeleteAuction(int id, string userId);

        Task<ViewAuctionDto> GetAuction(int id, string userid);

    }
}