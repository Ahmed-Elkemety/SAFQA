using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.BLL.Managers.SellerAppManager.BidService;
using System.Security.Claims;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TotalBidController : ControllerBase
    {
        private readonly IBidManager _bidManager;

        public TotalBidController(IBidManager bidManager)
        {
            _bidManager = bidManager;
        }

        [HttpGet("seller")]
        public async Task<IActionResult> TotalSellerBids()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var bids = await _bidManager.GetSellerBids(userId);
            return Ok(bids);
        }

        [Authorize]
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetBidsByCategory(int categoryId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var bids = await _bidManager.GetBidsByCategory(userId, categoryId);

            return Ok(bids);
        }
    }
}