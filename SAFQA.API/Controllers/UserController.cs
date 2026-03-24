using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Managers.UserAppManager;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "USER")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _homeService;

        public UserController(IUserService homeService)
        {
            _homeService = homeService;
        }

        [HttpGet ("Home")]
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
