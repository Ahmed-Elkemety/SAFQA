using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Managers.SellerAppManager.SellerDashboard;

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

        // GET: api/Auction/all/5
        [HttpGet("all/{sellerId}")]
        public IActionResult GetAllAuctions(int sellerId)
        {
            var auctions = _auctionManager.GetAllAuctions(sellerId).ToList();
            return Ok(auctions);
        }

        // GET: api/Auction/active/5
        [HttpGet("active/{sellerId}")]
        public IActionResult GetActiveAuctions(int sellerId)
        {
            var activeAuctions = _auctionManager.GetActiveAuctions(sellerId).ToList();
            return Ok(activeAuctions);
        }

        // GET: api/Auction/total/5
        [HttpGet("total/{sellerId}")]
        public async Task<IActionResult> GetTotalAuctions(int sellerId)
        {
            var total = await _auctionManager.GetTotalAuctionsAsync(sellerId);
            return Ok(total);
        }
    }
}
