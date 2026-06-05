using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.AnnouncementDto;
using SAFQA.BLL.Managers.AdminService;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AnnouncementController(IAdminService adminService)
        {
            _adminService = adminService;
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
    }
}
