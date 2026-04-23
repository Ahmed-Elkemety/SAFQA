using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Managers.UserAppManager.OrderService;
using System.Security.Claims;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "USER")]
        [HttpGet("delivered")]
        public async Task<IActionResult> GetDeliveredOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var orders = await _orderService.GetUserDeliveredOrders(userId);

            return Ok(orders);
        }

        [Authorize(Roles = "USER")]
        [HttpGet("in-progress")]
        public async Task<IActionResult> GetInProgressOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token or user not found");

            var result = await _orderService.GetUserInProgressOrders(userId);

            return Ok(result);
        }
    }
}
