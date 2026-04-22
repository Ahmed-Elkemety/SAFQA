using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.AccountDto.Forget_password;
using SAFQA.BLL.Dtos.DeliveryDto;
using SAFQA.BLL.Managers.DeliveryAppManager;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryService _delivery;

        public DeliveryController(IDeliveryService delivery)
        {
            _delivery = delivery;
        }

        [HttpPost("request-login-otp")]
        public async Task<IActionResult> RequestLoginOtp([FromBody] RequestOtpDto dto)
        {
            var result = await _delivery.RequestLoginOtpAsync(dto.Email);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("verify-login-otp")]
        public async Task<IActionResult> VerifyLoginOtp([FromBody] VerifyOtpDto dto)
        {
            var result = await _delivery.VerifyLoginOtpAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "USER")]
        [HttpGet("my-deliveries")]
        public async Task<IActionResult> GetMyDeliveries()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _delivery.GetSellerDeliveries(userId);

            if (!result.Item1.IsSuccess)
                return BadRequest(result.Item1);

            return Ok(new
            {
                result.Item1,
                result.Item2
            });
        }

        [Authorize(Roles = "USER")]
        [HttpPut("step-2/{auctionId}")]
        public async Task<IActionResult> Step2(int auctionId)
        {
            var result = await _delivery.Step2Async(auctionId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "USER")]
        [HttpPut("step-3")]
        public async Task<IActionResult> Step3([FromBody] Step3Dto dto)
        {
            var result = await _delivery.Step3Async(dto.AuctionId, dto.Contact);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "USER")]
        [HttpPut("step-4")]
        public async Task<IActionResult> Step4([FromForm] Step4Dto dto)
        {
            var result = await _delivery.Step4Async(dto.AuctionId, dto.Image);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "USER")]
        [HttpPut("step-5/{auctionId}")]
        public async Task<IActionResult> Step5(int auctionId)
        {
            var result = await _delivery.Step5Async(auctionId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
