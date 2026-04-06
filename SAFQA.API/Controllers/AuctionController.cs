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


        [HttpGet("Top Buyers")]
        public async Task<ActionResult<List<TopCustomerDto>>> GetTopCustomers()
        {
            var topCustomers = await _auctionManager.GetTopCustomers();

            if (topCustomers == null || topCustomers.Count == 0)
                return NotFound("No customers found.");

            return Ok(topCustomers);
        }

        [HttpGet("total-auctions")]
        public async Task<IActionResult> GetTotalAuctions()
        {
            int total = await _auctionManager.GetTotalAuctions();
            return Ok(new { totalAuctions = total });
        }

        [HttpGet("active-auctions")]
        public async Task<IActionResult> GetActiveAuctionsCount()
        {
            int count = await _auctionManager.GetActiveAuctionsCount();
            return Ok(new { activeAuctions = count });
        }

        [HttpGet("expired-auctions")]
        public async Task<IActionResult> GetExpiredAuctionsCount()
        {
            int count = await _auctionManager.GetExpiredAuctionsCount();
            return Ok(new { expiredAuctions = count });
        }

        [HttpGet("upcoming-auctions")]
        public async Task<IActionResult> GetUpcomingAuctionsCount()
        {
            int count = await _auctionManager.GetUpcomingAuctionsCount();
            return Ok(new { upcomingAuctions = count });
        }

       
        [HttpGet("top-profitable")]
        public async Task<IActionResult> GetTopProfitableAuctions([FromQuery] int sellerId, [FromQuery] int categoryId)
        {
            var result = await _auctionManager
                .GetTopProfitableAuctions(sellerId, categoryId);

            return Ok(result);
        }

        [HttpGet("auctions-bids/{sellerId}")]
        public async Task<ActionResult> GetSellerAuctionsBids(int sellerId)
        {
            var result = await _auctionManager.GetSellerAuctionsBids(sellerId);

            if (result == null || !result.Any())
            {
                return NotFound($"No auctions with bids found for seller ID {sellerId}");
            }

            return Ok(result);
        }
        
        [HttpGet("{sellerId}/category-percentages")]
        public async Task<IActionResult> GetCategoryPercentages(int sellerId)
        {
            var result = await _auctionManager.GetCategoryPercentageBySeller(sellerId);

            if (result == null || !result.Any())
                return NotFound("No items or auctions found for this seller");

            return Ok(result);
        }
    }
}