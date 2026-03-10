using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Dtos.UserAppDto.HomeDto;

namespace SAFQA.BLL.Managers.UserAppManager.Home_Manager
{
    public interface IHomeService
    {
        Task<List<TrendingAuctionDto>> GetTrendingAuctionsAsync();
        Task<List<CategoryWithDetailsDto>> GetCategoriesWithDetailsAsync();
    }
}
