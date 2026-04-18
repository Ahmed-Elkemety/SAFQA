using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Managers.RecommendationAI;
using SAFQA.DAL.Database;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {

        private readonly IRecommendationAI _recommendationService;
        private readonly SAFQA_Context _context;

        public AIController(IRecommendationAI recommendationService,SAFQA_Context Context)
        {
            _recommendationService = recommendationService;
            _context = Context;
        }

        [HttpGet("my-recommendations/{userId}")]
        public async Task<IActionResult> GetUserRecs(string userId)
        {
            var recommendations = await _recommendationService.GetRecommendationsAsync(userId);

            if (recommendations == null || recommendations.Count == 0)
            {
                return Ok(await _context.Auctions.OrderByDescending(a => a.CreatedAt).Take(10).ToListAsync());
            }

            return Ok(recommendations);
        }
    }
}
