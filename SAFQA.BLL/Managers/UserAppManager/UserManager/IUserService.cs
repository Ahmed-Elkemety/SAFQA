using SAFQA.BLL.Dtos.UserAppDto.HomeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SAFQA.BLL.Help.Helper;
namespace SAFQA.BLL.Managers.UserAppManager
{
    public interface IUserService
    {
        Task<List<TrendingAuctionDto>> GetTrendingAuctionsAsync();
        Task<List<CategoryWithDetailsDto>> GetCategoriesWithDetailsAsync();
        Task<int> GetTotalUsersAsync();
        Task<int> GetActiveUsersCountAsync();
        Task<int> GetBlockedUsersCountAsync();
        PagedResult<UserListDto> GetUsers(int page, int pageSize);
        bool ChangeStatus(string userId);
    }
}
