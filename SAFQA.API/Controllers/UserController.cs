using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.UserAppDto.AccountDto;
using SAFQA.BLL.Managers.UserAppManager;
using SAFQA.BLL.Managers.UserAppManager.AuctionManager;
using SAFQA.BLL.Managers.UserAppManager.UserManager;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "USER")]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuctionManagerU _managerU;

        public UserController(IUserService homeService ,IAuctionManagerU managerU)
        {
            _userService = homeService;
            _managerU = managerU;
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

        [HttpGet("ending-soon")]
        public async Task<IActionResult> GetEndingSoon(int page = 1, int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _managerU.GetEndingSoonAsync( userId,page, pageSize);
            return Ok(result);
        }

        [HttpGet("trending")]
        public async Task<IActionResult> GetTrending(int page = 1, int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _managerU.GetTrendingAsync(userId,page, pageSize);
            return Ok(result);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _managerU.GetCategoriesAsync(userId);
            return Ok(result);
        }

    }
}