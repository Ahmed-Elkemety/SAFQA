using System.Security.Claims;
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

        [HttpGet("recommendations")]
        public async Task<IActionResult> GetRecommendations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _recommendationService.GetRecommendations(userId);

            return Ok(result);
        }
    }
}
