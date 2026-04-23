using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.UserAppDto.ReviewsDto;
using SAFQA.BLL.Managers.UserAppManager.ReviewService;
using System.Security.Claims;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize(Roles = "USER")]
        [HttpPost("add")]
        public async Task<IActionResult> AddReview([FromBody] AddReviewDto dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return Unauthorized();

                await _reviewService.AddReviewAsync(userId, dto);

                return Ok(new
                {
                    message = "Review added successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "USER")]
        [HttpGet("{sellerId}")]
        public IActionResult GetSellerReviews(int sellerId)
        {
            var result = _reviewService.GetSellerReviews(sellerId);

            if (result == null)
                return NotFound(new { message = "Seller not found" });

            return Ok(result);
        }
    }
}