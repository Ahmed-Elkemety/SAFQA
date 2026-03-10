using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SAFQA.DAL.Dtos.AccountDto.Facebook;
using SAFQA.DAL.Dtos.AccountDto.Forget_password;
using SAFQA.DAL.Dtos.AccountDto.Google;
using SAFQA.DAL.Dtos.AccountDto.User;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.BLL.Managers.AccountManager.OAuth;

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

        [HttpPost("resend")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp([FromBody] RequestResetDto dto)
        {
            var result = await _authUser.ResendRegistrationOtpAsync(dto.Email);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
