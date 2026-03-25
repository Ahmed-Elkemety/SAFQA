using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.AccountDto.Seller;
using SAFQA.BLL.Managers.SellerAppManager;

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

        [HttpPost("CreateSeller")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateSeller(CreateSellerDto dto)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User Not Found");

            try
            {
                await _sellerService.CreateSellerAsync(userId, dto);
                return Ok(new { Message = "Seller created successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "Something went wrong" });
            }
        }

        [AllowAnonymous]
        [HttpPost("{sellerId}/personal-verification")]
        public async Task<IActionResult> PersonalVerification(int sellerId, PersonalSellerDto dto)
        {
            var result = await _sellerService.UploadPersonalDocsAsync(sellerId, dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("{sellerId}/business-verification")]
        public async Task<IActionResult> BusinessVerification(int sellerId, BusinessSellerDto dto)
        {
            var result = await _sellerService.UploadBusinessDocsAsync(sellerId, dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("basic")]
        [Authorize(Roles = "SELLER")] // لازم التوكن
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
        public async Task<IActionResult> GetPendingSellers()
        {
            var count = await _sellerService.GetPendingSellersCount();
            return Ok(new { pendingSellers = count });
        }
    }
}
