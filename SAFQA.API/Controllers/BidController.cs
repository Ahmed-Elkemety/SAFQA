using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.UserAppDto.BidDto;
using SAFQA.BLL.Dtos.UserAppDto.ProxyBidding;
using SAFQA.BLL.Managers.UserAppManager.BidService;
using SAFQA.BLL.Managers.UserAppManager.ProxyBidingService;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "USER")]
    public class BidController : ControllerBase
    {
        private readonly IBidService _bidService;
        private readonly IProxyService _proxy;

        public BidController(IBidService bidService ,IProxyService proxy)
        {
            _bidService = bidService;
            _proxy = proxy;
        }

        [HttpPost("manual")]
        public async Task<IActionResult> ManualBid(BidRequestDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _bidService.PlaceManualBid(userId, dto.AuctionId, dto.Amount);

            return Ok(new { message = "Bid placed" });
        }

        [HttpPost("activate/{auctionId}")]
        public async Task<IActionResult> Activate(int auctionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _proxy.ActivateAsync(auctionId, userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("deactivate/{auctionId}")]
        public async Task<IActionResult> Deactivate(int auctionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _proxy.DeactivateAsync(auctionId, userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("create-Proxy")]
        public async Task<IActionResult> Create(CreateProxyDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _proxy.CreateAsync(dto, userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("update-Proxy")]
        public async Task<IActionResult> Update(UpdateProxyDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _proxy.UpdateAsync(dto, userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
