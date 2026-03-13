using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Managers.SellerAppManager.SellerDashboard;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionManager _transactionManager;

        public TransactionController(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }

        // GET: api/Transaction/pending/5
        [HttpGet("pending/{sellerId}")]
        public IActionResult GetPendingPayments(int sellerId)
        {
            var pendingPayments = _transactionManager.GetPendingPayments(sellerId).ToList();
            return Ok(pendingPayments);
        }

        // GET: api/Transaction/all/5
        [HttpGet("all/{sellerId}")]
        public IActionResult GetSellerPayments(int sellerId)
        {
            var sellerPayments = _transactionManager.GetSellerPayments(sellerId).ToList();
            return Ok(sellerPayments);
        }

        // GET: api/Transaction/revenue/5
        [HttpGet("revenue/{sellerId}")]
        public async Task<IActionResult> GetTotalRevenue(int sellerId)
        {
            var totalRevenue = await _transactionManager.GetTotalRevenueAsync(sellerId);
            return Ok(totalRevenue);
        }
    }
}
