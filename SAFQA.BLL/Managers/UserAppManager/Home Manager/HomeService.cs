using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Dtos.UserAppDto.HomeDto;
using SAFQA.DAL.Repository.Home;

namespace SAFQA.BLL.Managers.UserAppManager.Home_Manager
{
    public class HomeService : IHomeService
    {
        private readonly IHomeRepository _homeRepository;

        public HomeService(IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
        }

        public async Task<List<TrendingAuctionDto>> GetTrendingAuctionsAsync()
        {
            return await _homeRepository.GetTrendingAuctionsAsync();
        }

        public async Task<List<CategoryWithDetailsDto>> GetCategoriesWithDetailsAsync()
        {
            return await _homeRepository.GetCategoriesWithDetailsAsync();
        }
    }

}
