using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Managers.UserAppManager;
using SAFQA.BLL.Managers.UserAppManager.UserManager;

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


        [Authorize(Roles = "ADMIN")]
        [HttpGet("total-users")]
        public async Task<IActionResult> GetTotalUsers()
        {
            var totalUsers = await _homeService.GetTotalUsersAsync();

            return Ok(new
            {
                TotalUsers = totalUsers
            });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("active-count")]
        public async Task<IActionResult> GetActiveUsersCount()
        {
            int count = await _homeService.GetActiveUsersCountAsync();
            return Ok(new { ActiveUsersCount = count });
        }


        [Authorize(Roles = "ADMIN")]
        [HttpGet("blocked-count")]
        public async Task<IActionResult> GetBlockedUsersCount()
        {
            int blockedCount = await _homeService.GetBlockedUsersCountAsync();
            return Ok(new { Count = blockedCount });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public IActionResult GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = _homeService.GetUsers(page, pageSize);
            return Ok(result);
        }


        [Authorize(Roles = "ADMIN")]
        [HttpPost("{userId}/change-status")]
        public IActionResult ChangeStatus(string userId)
        {
            var result = _homeService.ChangeStatus(userId);

            if (!result)
                return NotFound("User not found");

            return Ok(new
            {
                message = "User status updated successfully"
            });
        }
    }
}