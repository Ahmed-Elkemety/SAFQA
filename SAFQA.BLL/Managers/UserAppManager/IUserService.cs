using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Dtos.UserAppDto.HomeDto;
namespace SAFQA.BLL.Managers.UserAppManager
{
    public interface IUserService
    {
        //Home
        Task<List<TrendingAuctionDto>> GetTrendingAuctionsAsync();
        Task<List<CategoryWithDetailsDto>> GetCategoriesWithDetailsAsync();
    }
}
