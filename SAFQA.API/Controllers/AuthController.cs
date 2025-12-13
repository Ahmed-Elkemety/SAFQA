using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.AccountDto;
using SAFQA.BLL.Managers.AccountManager;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthUser _authUser;

        public AuthController(IAuthUser authUser)
        {
            _authUser = authUser;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var deviceId = Request.Headers["DeviceId"].FirstOrDefault() ?? Guid.NewGuid().ToString();


            var result = await _authUser.LoginAsync(dto, deviceId);
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

    }
}
