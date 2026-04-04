using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.SellerAppDto.NotificationDto;
using SAFQA.BLL.Managers.SellerAppManager.Notification;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SELLER")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationManager _manager;

        public NotificationsController(INotificationManager manager)
        {
            _manager = manager;
        }

        [HttpGet("Get-Notifications")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _manager.GetUserNotificationsAsync(userId);

            return Ok(result);
        }

        [HttpDelete("Delete-SelectedNotification")]
        public async Task<IActionResult> DeleteNotifications([FromBody] DeleteNotificationsDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _manager.DeleteNotificationsAsync(dto, userId);

            return Ok(new { success = result });
        }
    }
}
