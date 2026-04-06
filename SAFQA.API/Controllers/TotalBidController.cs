using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.BLL.Managers.SellerAppManager.SellerDashboard.BidService;

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
    }
}