using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.AnnouncementDto;
using SAFQA.BLL.Dtos.UserAppDto.AccountDto;
using SAFQA.BLL.Dtos.UserAppDto.FollowDto;
using SAFQA.BLL.Managers.AdminService;
using SAFQA.BLL.Managers.UserAppManager;
using SAFQA.BLL.Managers.UserAppManager.AuctionManager;
using SAFQA.BLL.Managers.UserAppManager.InteractionService;
using SAFQA.BLL.Managers.UserAppManager.UserManager;
using System.Security.Claims;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "USER")]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuctionManagerU _managerU;
        private readonly IUserInteractionService _interactionService;
        private readonly IUserService _user;
        private readonly IAdminService _adminService;

        public UserController(IAdminService adminservice, IUserService homeService ,IAuctionManagerU managerU , IUserInteractionService interactionService,IUserService user)
        {
            _userService = homeService;
            _managerU = managerU;
            _interactionService = interactionService;
            _user = user;
            _adminService = adminservice;
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

        // ===============================
        // ADD FAVORITE
        // ===============================
        [HttpPost("add-favorite/{auctionId}")]
        public async Task<IActionResult> AddFavorite(int auctionId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _interactionService.AddFavoriteAsync(auctionId, userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // ===============================
        // ADD VIEW
        // ===============================
        [HttpPost("add-view/{auctionId}")]
        public async Task<IActionResult> AddView(int auctionId, [FromQuery] string deviceType)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _interactionService.AddViewAsync(auctionId, userId, deviceType);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("Follow")]
        public async Task<IActionResult> Follow(FollowSellerDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _user
                .FollowSeller(userId, dto.SellerId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("Unfollow/{sellerId}")]
        public async Task<IActionResult> Unfollow(int sellerId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _user
                .UnfollowSeller(userId, sellerId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("RemoveFavorite/{auctionId}")]
        public async Task<IActionResult> RemoveFavorite(int auctionId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _user.RemoveFavorite(userId, auctionId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("send-global")]
        public async Task<IActionResult> SendGlobalAnnouncement([FromBody] SendAnnouncementDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request");
            try
            {
                await _adminService.SendGlobalAnnouncement(dto);
                return Ok(new
                {
                    message = "Announcement sent successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public IActionResult GetUser([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = _userService.GetUsers(page, pageSize);
            return Ok(result);
        }
    }
}