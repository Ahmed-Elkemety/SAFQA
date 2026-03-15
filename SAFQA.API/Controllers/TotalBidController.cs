using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Managers.Dtos;
using SAFQA.BLL.Managers.SellerAppManager.SellerDashboard;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TotalBidController : ControllerBase
    {
        private readonly IBidManager _bidManager;

        public TotalBidController(IBidManager bidManager)
        {
            _bidManager = bidManager;
        }

        // GET: api/Bid/seller/5
        [HttpGet("seller/{sellerId}")]
        public async Task<IActionResult> TotalSellerBids(int sellerId)
        {
            var bids = await _bidManager.GetSellerBids(sellerId);
            return Ok(bids);
        }

        // GET: api/Bid/category/5/2
        [HttpGet("category/{sellerId}/{categoryId}")]
        public async Task<IActionResult> GetBidsByCategory(int sellerId, int categoryId)
        {
            var bids = await _bidManager.GetBidsByCategory(sellerId, categoryId);
            return Ok(bids);
        }

        [HttpGet("Top4Auctions/{sellerId}")]
        public async Task<ActionResult<List<AuctionBidsDto>>> GetTop4Auctions(int sellerId)
        {
            var result = await _bidManager.GetTop4AuctionsBySeller(sellerId);
            return Ok(result);
        }
    }
}