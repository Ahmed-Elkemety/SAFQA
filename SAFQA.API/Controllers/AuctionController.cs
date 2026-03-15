using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.BLL.Managers.SellerAppManager.SellerDashboard.AuctionService;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionManager _auctionManager;

        public AuctionController(IAuctionManager auctionManager)
        {
            _auctionManager = auctionManager;
        }

        // GET: api/Auction/active/5
        [HttpGet("active/{sellerId}")]
        public async Task<IActionResult> GetActiveAuctions(int sellerId)
        {
            var activeAuctions = await _auctionManager.GetActiveSellerAuctions(sellerId);
            return Ok(activeAuctions);
        }

        // GET: api/Auction/total/5
        [HttpGet("total/{sellerId}")]
        public async Task<IActionResult> GetTotalAuctions(int sellerId)
        {
            var total = await _auctionManager.GetTotalSellerAuctions(sellerId);
            return Ok(total);
        }

        [HttpGet("{sellerId}")]
        public async Task<ActionResult<List<SellerWinnerDto>>> GetSellerWinners(int sellerId)
        {
            if (sellerId <= 0)
                return BadRequest("SellerId must be greater than zero.");

            var winners = await _auctionManager.GetSellerWinnersAsync(sellerId);

            if (winners == null || winners.Count == 0)
                return NotFound("No winners found for this seller.");

            return Ok(winners);
        }
    }
}