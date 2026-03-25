using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Managers.UserAppManager;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly IUserService _homeService;

        public UserController(IUserService homeService)
        {
            _homeService = homeService;
        }

        [HttpGet("Home")]
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

        [HttpGet("total-users")]
        public async Task<IActionResult> GetTotalUsers()
        {
            var totalUsers = await _homeService.GetTotalUsersAsync();

            return Ok(new
            {
                TotalUsers = totalUsers
            });
        }


        [HttpGet("active-count")]
        public async Task<IActionResult> GetActiveUsersCount()
        {
            int count = await _homeService.GetActiveUsersCountAsync();
            return Ok(new { ActiveUsersCount = count });
        }

        [HttpGet("blocked-count")]
        public async Task<IActionResult> GetBlockedUsersCount()
        {
            int blockedCount = await _homeService.GetBlockedUsersCountAsync();
            return Ok(new { Count = blockedCount });
        }

        
    }
}