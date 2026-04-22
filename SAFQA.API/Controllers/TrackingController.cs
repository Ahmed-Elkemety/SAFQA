using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Managers.UserAppManager.TrackingService;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        private readonly TrackingService _orderTrackingService;

        public TrackingController(TrackingService orderTrackingService)
        {
            _orderTrackingService = orderTrackingService;
        }
        
        
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
