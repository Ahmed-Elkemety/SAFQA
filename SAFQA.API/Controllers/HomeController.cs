using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Managers.UserAppManager.Home_Manager;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHomeData()
        {
            var trending = await _homeService.GetTrendingAuctionsAsync();
            var categories = await _homeService.GetCategoriesWithDetailsAsync();

            return Ok(new
            {
                TrendingAuctions = trending,
                Categories = categories
            });
        }
    }
}
