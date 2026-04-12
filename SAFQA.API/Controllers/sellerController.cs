using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.AccountDto.Seller;
using SAFQA.BLL.Dtos.SellerAppDto.BussinessAccountDto;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.BLL.Managers.SellerAppManager.SellerManager;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class sellerController : ControllerBase
    {
        private readonly IsellerManager _sellerService;
        
        public sellerController(IsellerManager sellerService)
        {
            _sellerService = sellerService;
        }

        [Authorize(Roles = "USER")]
        [HttpPost("CreateSeller")]
        [Consumes("multipart/form-data")]

        public async Task<IActionResult> CreateSeller(CreateSellerDto dto)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var deviceId = Request.Headers["DeviceId"].FirstOrDefault() ?? Guid.NewGuid().ToString();


            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User Not Found");

            try
            {
                var result = await _sellerService.CreateSellerAsync(userId, dto , deviceId);

                if (!result.IsSuccess)
                {
                    if (result.Message == "Seller already exists")
                        return Conflict(result);

                    return BadRequest(result);
                }

                return Ok(result); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResult
                {
                    IsSuccess = false,
                    Message = "Something went wrong",
                    Errors = new List<string> { ex.Message, ex.InnerException?.Message ?? "" }
                });
            }
        }

        [Authorize(Roles = "SELLER")]
        [HttpPost("personal-verification")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PersonalVerification(PersonalSellerDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User Not Found");

            var result = await _sellerService.UploadPersonalDocsAsync(userId, dto);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "SELLER")]
        [HttpPost("business-verification")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> BusinessVerification(BusinessSellerDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User Not Found");

            var result = await _sellerService.UploadBusinessDocsAsync(userId, dto);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("Home")]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> GetMySellerHome()
        {
            // جلب UserId من التوكن
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Token Dont Contain UserID");

            var seller = await _sellerService.GetMySellerHomeAsync(userId);

            if (seller == null)
                return NotFound(new { message = "Seller not found" });

            return Ok(seller);
        }

        [HttpGet("business-account")]
        [Authorize(Roles = "SELLER")]
        public async Task<IActionResult> GetBusinessAccount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User Not Found");

            var result = await _sellerService.GetBusinessAccountAsync(userId);

            if (result == null)
                return NotFound("Seller not found");

            return Ok(result);
        }

        [Authorize(Roles = "SELLER")]
        [HttpPut("edit-profile")]
        public async Task<IActionResult> EditProfile(EditSellerProfileDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new AuthResult
                {
                    IsSuccess = false,
                    Message = "Unauthorized"
                });

            var result = await _sellerService.EditProfile(userId, dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "SELLER")]
        [HttpPost("upgrade")]
        public async Task<IActionResult> Upgrade([FromBody] UpgradeRequestDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _sellerService.UpgradeSellerAsync(userId, dto.UpgradeType);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("total-sellers")]
        public async Task<IActionResult> GetTotalSellers()
        {
            var count = await _sellerService.GetTotalSellersCount();
            return Ok(new { totalSellers = count });
        }
        
        [HttpGet("verified-sellers")]
        public async Task<IActionResult> GetVerifiedSellers()
        {
            var count = await _sellerService.GetVerifiedSellersCount();
            return Ok(new { verifiedSellers = count });
        }

        [HttpGet("pending-sellers")]
        public async Task<IActionResult> GetPendingSellerscount()
        {
            var count = await _sellerService.GetPendingSellersCount();
            return Ok(new { pendingSellers = count });
        }



        [HttpGet("pending")]
        public IActionResult GetPendingSellers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var result = _sellerService.GetPendingSellers(page, pageSize);

            return Ok(result);
        }

        [HttpPost("approve/{userId}")]
        public async Task<IActionResult> ApproveSeller(string userId)
        {
            var result = await _sellerService.ApproveSeller(userId);

            if (!result)
                return NotFound("Seller not found");

            return Ok(new { message = "Seller approved successfully" });
        }

        [HttpPost("reject/{userId}")]
        public async Task<IActionResult> RejectSeller(string userId)
        {
            var result = await _sellerService.RejectSeller(userId);

            if (!result)
                return NotFound("Seller not found");

            return Ok(new { message = "Seller rejected successfully" });
        }

        [HttpGet]
        public IActionResult GetAllSellers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var result = _sellerService.GetAllSellers(page, pageSize);

            return Ok(result);
        }

        [HttpPut("suspend/{userId}")]
        public async Task<IActionResult> SuspendSeller(string userId)
        {
            var result = await _sellerService.SuspendSeller(userId);

            if (!result)
                return NotFound(new { message = "Seller or User not found" });

            return Ok(new { message = "Seller suspended successfully" });
        }

        [HttpPut("restore/{userId}")]
        public async Task<IActionResult> RestoreSeller(string userId)
        {
            var result = await _sellerService.RestoreSeller(userId);

            if (!result)
                return NotFound(new { message = "Seller or User not found" });

            return Ok(new { message = "Seller restored successfully" });
        }

        [HttpGet("seller/{userId}")]
        public IActionResult GetSellerDetails(string userId)
        {
            var result = _sellerService.GetSellerDetails(userId);

            if (result == null)
                return NotFound(new { message = "Seller not found" });

            return Ok(result);
        }
    }
}
