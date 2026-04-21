using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.UserAppDto.DisputeDto;
using SAFQA.BLL.Managers.UserAppManager.DisputeService;
using System.Security.Claims;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisputeController : ControllerBase
    {
        private readonly IDisputeService _disputeService;

        public DisputeController(IDisputeService disputeService)
        {
            _disputeService = disputeService;
        }
        [Authorize("USER")]
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

        [Authorize( Roles = "USER")]
        [HttpGet("my-reports")]
        public async Task<IActionResult> GetMyReports()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _disputeService.GetUserReports(userId);

            if (!result.Item1.IsSuccess)
                return BadRequest(result.Item1);

            return Ok(new
            {
                result.Item1,
                Reports = result.Item2
            });
        }
    }
}