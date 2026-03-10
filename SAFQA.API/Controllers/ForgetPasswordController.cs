using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAFQA.DAL.Dtos.AccountDto.Forget_password;
using SAFQA.BLL.Managers.AccountManager.Forget_Password;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgetPasswordController : ControllerBase
    {
        private readonly IForgetPassword _forgetPassword;

        public ForgetPasswordController(IForgetPassword forgetPassword)
        {
            _forgetPassword = forgetPassword;
        }

        // 1️⃣ ارسال OTP
        [HttpPost("request")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestOtp([FromBody] RequestResetDto dto)
        {
            var result = await _forgetPassword.RequestPasswordResetAsync(dto.Email);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // 2️⃣ تأكيد OTP
        [HttpPost("verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var result = await _forgetPassword.VerifyOtpAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // 3️⃣ تغيير الباسورد
        [HttpPost("reset")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _forgetPassword.ResetPasswordAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("resend")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp([FromBody] RequestResetDto dto)
        {
            var result = await _forgetPassword.ResendOtpAsync(dto.Email);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("signout-all")]
        [Authorize]
        public async Task<IActionResult> SignOutAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _forgetPassword.SignOutAllDevicesAsync(userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

    }
}
