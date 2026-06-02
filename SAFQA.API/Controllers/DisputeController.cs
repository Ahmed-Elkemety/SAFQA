using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.UserAppDto.DisputeDto;
using SAFQA.BLL.Managers.AdminService;
using SAFQA.BLL.Managers.UserAppManager.DisputeService;
using System.Security.Claims;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisputeController : ControllerBase
    {
        private readonly IDisputeService _disputeService;
        private readonly IAdminService _adminService;
        public DisputeController(IDisputeService disputeService, IAdminService adminService)
        {
            _adminService = adminService;
            _disputeService = disputeService;
        }


        [Authorize(Roles = "USER")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateDispute([FromBody] CreateDisputeDto dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User not authenticated");

                var result = await _disputeService.CreateDispute(userId, dto);

                return Ok(new
                {
                    message = "Dispute created successfully",
                    data = result
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

        [Authorize(Roles = "USER")]
        [HttpGet("my-reports")]
        public async Task<IActionResult> GetMyReports(
             int page = 1,
             int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _disputeService
                .GetUserReports(userId, page, pageSize);

            if (!result.Item1.IsSuccess)
                return BadRequest(result.Item1);

            return Ok(new
            {
                result.Item1,
                Reports = result.Item2
            });
        }

        [Authorize(Roles = "USER")]
        [HttpGet("tracking/{disputeId}")]
        public async Task<IActionResult> GetDisputeTracking(int disputeId)
        {
            try
            {
                var result = await _disputeService.GetDisputeTracking(disputeId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("cancel/{disputeId}")]
        public async Task<IActionResult> CancelDispute(int disputeId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User not found");

                await _disputeService.CancelDisputeAsync(disputeId, userId);

                return Ok(new
                {
                    message = "Dispute cancelled successfully"
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

        [HttpGet("escalated-cards")]
        public IActionResult GetEscalatedCards()
        {
            var result = _adminService.GetEscalatedCards();

            if (result == null || !result.Any())
            {
                return NotFound("No escalated disputes found.");
            }

            return Ok(result);
        }

        [HttpGet("chat/{disputeId}")]
        public IActionResult GetDisputeChat(int disputeId)
        {
            var result = _adminService.GetDisputeChat(disputeId);

            if (result == null)
                return NotFound("Conversation not found for this dispute");

            return Ok(result);
        }

        [HttpGet("disputes/{disputeId}/details")]
        public IActionResult GetDisputeDetails(int disputeId)
        {
            var result = _disputeService.GetDisputeDetails(disputeId);

            if (result == null)
            {
                return NotFound(new
                {
                    Message = "Dispute not found."
                });
            }

            return Ok(result);
        }
    }
}