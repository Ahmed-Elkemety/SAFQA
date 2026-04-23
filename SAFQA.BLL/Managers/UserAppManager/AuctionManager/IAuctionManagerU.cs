using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Dtos.SellerAppDto.AuctionDto.AuctionProcessDtos;
using SAFQA.BLL.Dtos.UserAppDto.AuctionDto;
using SAFQA.BLL.Dtos.UserAppDto.HomeDto;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.DAL.Models;

namespace SAFQA.BLL.Managers.UserAppManager.AuctionManager
{
    public interface IAuctionManagerU
    {
        Task<AuthResult> ReportAuctionAsync(string userId, CreateReportDto dto);
        Task<(AuthResult, object)> GetAuctionsByCategory(int categoryId, string userId, int pageNumber, int pageSize, AuctionQueryDto queryDto);
        Task<(AuthResult, List<FavoritesDto>, int)> GetFavoriteAuctions(string userId, int pageNumber, int pageSize , AuctionQueryDto queryDto);
        Task CalculateHotScoresAsync();
        Task<(AuthResult, AuctionDetailsDto?)> GetAuctionDetails(int auctionId, string userId);
        Task<AuthResult> CheckSecurityDeposit(int auctionId, string userId);

        Task UpdateAuctionStatusesAsync();
        Task<List<EndingSoonDto>> GetEndingSoonAsync(string userId, int page, int pageSize);
        Task<List<TrendingDto>> GetTrendingAsync(string userId, int page, int pageSize);
        Task<List<Dtos.UserAppDto.HomeDto.CategoryDto>> GetCategoriesAsync(string userId);
    }
}
