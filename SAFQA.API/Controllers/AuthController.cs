using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.AccountDto.Facebook;
using SAFQA.BLL.Dtos.AccountDto.Forget_password;
using SAFQA.BLL.Dtos.AccountDto.Google;
using SAFQA.BLL.Dtos.AccountDto.User;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.BLL.Managers.AccountManager.OAuth;
using System.Security.Claims;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthUser _authUser;
        private readonly IOAuth _oAuth;

        public AuthController(IAuthUser authUser , IOAuth oAuth)
        {
            _authUser = authUser ;
            _oAuth = oAuth;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var deviceId = Request.Headers["DeviceId"].FirstOrDefault() ?? Guid.NewGuid().ToString();


            var result = await _authUser.RegisterAsync(dto, deviceId);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto dto)
        {
            var result = await _authUser.ConfirmEmailAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(result.Message);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var deviceId = Request.Headers["DeviceId"].FirstOrDefault() ?? Guid.NewGuid().ToString();


            var result = await _authUser.LoginAsync(dto, deviceId);
            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result);
        }

        [HttpPost("google")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleDto dto)
        {
            var deviceId = Request.Headers["DeviceId"].FirstOrDefault() ?? Guid.NewGuid().ToString();

            var result = await _oAuth.GoogleLoginAsync(dto.IdToken, deviceId);

            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result);
        }

        [HttpPost("facebook")]
        [AllowAnonymous]
        public async Task<IActionResult> FacebookLogin([FromBody] FacebookLoginDto dto)
        {
            var deviceId = Request.Headers["DeviceId"].FirstOrDefault() ?? Guid.NewGuid().ToString();
            var result = await _oAuth.FacebookLoginAsync(dto.AccessToken, deviceId);

            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var deviceId = Request.Headers["DeviceId"].FirstOrDefault();


            var result = await _authUser.RefreshTokenAsync(refreshToken,deviceId);
            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result);
        }

        [HttpPost("resendRegistrationOtp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendRegistrationOtp([FromBody] RequestResetDto dto)
        {
            var result = await _authUser.ResendRegistrationOtpAsync(dto.Email);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // 1️⃣ ارسال OTP
        [HttpPost("request")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestOtp([FromBody] RequestResetDto dto)
        {
            var result = await _authUser.RequestPasswordResetAsync(dto.Email);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // 2️⃣ تأكيد OTP
        [HttpPost("verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var result = await _authUser.VerifyOtpAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // 3️⃣ تغيير الباسورد
        [HttpPost("reset")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authUser.ResetPasswordAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("resendOtp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp([FromBody] RequestResetDto dto)
        {
            var result = await _authUser.ResendOtpAsync(dto.Email);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("signout-all")]
        [Authorize]
        public async Task<IActionResult> SignOutAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _authUser.SignOutAllDevicesAsync(userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

    }
}
