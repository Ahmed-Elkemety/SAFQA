using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Managers.SellerAppManager.SellerDashboard;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BidController : ControllerBase
    {
        private readonly IBidManager _bidManager;

        public BidController(IBidManager bidManager)
        {
            _bidManager = bidManager;
        }

        // GET: api/Bid/seller/5
        [HttpGet("seller/{sellerId}")]
        public IActionResult GetSellerBids(int sellerId)
        {
            var bids = _bidManager.GetSellerBids(sellerId).ToList();
            return Ok(bids);
        }

        // GET: api/Bid/category/5/2
        [HttpGet("category/{sellerId}/{categoryId}")]
        public IActionResult GetBidsByCategory(int sellerId, int categoryId)
        {
            var bids = _bidManager.GetBidsByCategory(sellerId, categoryId).ToList();
            return Ok(bids);
        }
    }
}