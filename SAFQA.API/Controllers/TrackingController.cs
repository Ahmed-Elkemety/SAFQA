using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Managers.UserAppManager.TrackingService;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        private readonly ITrackingService _orderTrackingService;

        public TrackingController(ITrackingService orderTrackingService)
        {
            _orderTrackingService = orderTrackingService;
        }

        [Authorize(Roles = "USER")]
        [HttpGet("{auctionId}")]
        public async Task<IActionResult> GetOrderTracking(int auctionId)
        {
            var result = await _orderTrackingService.GetOrderTrackingAsync(auctionId);

            if (result == null)
                return NotFound(new
                {
                    message = "Order not found"
                });

            return Ok(result);
        }
    }
}
