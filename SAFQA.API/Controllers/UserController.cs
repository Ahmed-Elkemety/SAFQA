using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.UserAppDto.AccountDto;
using SAFQA.BLL.Managers.UserAppManager;
using SAFQA.BLL.Managers.UserAppManager.UserManager;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "USER")]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService homeService)
        {
            _userService = homeService;
        }

        [HttpGet("Home")]
        public async Task<IActionResult> GetHomeData()
        {
            var trending = await _userService.GetTrendingAuctionsAsync();
            var categories = await _userService.GetCategoriesWithDetailsAsync();

            return Ok(new
            {
                TrendingAuctions = trending,
                Categories = categories
            });
        }


        [HttpGet("total-users")]
        public async Task<IActionResult> GetTotalUsers()
        {
            var totalUsers = await _userService.GetTotalUsersAsync();

            return Ok(new
            {
                TotalUsers = totalUsers
            });
        }

        [HttpGet("active-count")]
        public async Task<IActionResult> GetActiveUsersCount()
        {
            int count = await _userService.GetActiveUsersCountAsync();
            return Ok(new { ActiveUsersCount = count });
        }


        [HttpGet("blocked-count")]
        public async Task<IActionResult> GetBlockedUsersCount()
        {
            int blockedCount = await _userService.GetBlockedUsersCountAsync();
            return Ok(new { Count = blockedCount });
        }

        [HttpGet]
        public IActionResult GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = _userService.GetUsers(page, pageSize);
            return Ok(result);
        }


        [HttpPost("{userId}/change-status")]
        public IActionResult ChangeStatus(string userId)
        {
            var result = _userService.ChangeStatus(userId);

            if (!result)
                return NotFound("User not found");

            return Ok(new
            {
                message = "User status updated successfully"
            });
        }

        // 🟢 Get Profile
        [HttpGet("view-profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var (result, data) = await _userService.GetProfile(userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(new
            {
                result.IsSuccess,
                result.Message,
                data
            });
        }

        // 🟢 Get Account
        [HttpGet("view-account")]
        public async Task<IActionResult> GetAccount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var (result, data) = await _userService.GetAccount(userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(new
            {
                result.IsSuccess,
                result.Message,
                data
            });
        }

        // 🟢 Edit Account
        [HttpPut("edit-account")]
        public async Task<IActionResult> EditAccount([FromForm] EditAccountDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _userService.EditAccount(userId, dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}