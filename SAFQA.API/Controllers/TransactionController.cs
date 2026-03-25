using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.BLL.Managers.SellerAppManager.SellerDashboard.TransactionService;

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
        public async Task<IActionResult> GetPendingPayments(int sellerId)
        {
            var pendingPayments = await _transactionManager.GetTotalPendingPayments(sellerId);
            return Ok(pendingPayments);
        }

        // GET: api/Transaction/revenue/5
        [HttpGet("revenue/{sellerId}")]
        public async Task<IActionResult> GetTotalRevenue(int sellerId)
        {
            var totalRevenue = await _transactionManager.GetTotalRevenueAsync(sellerId);
            return Ok(totalRevenue);
        }

        [HttpGet("MonthlyRevenue/{sellerId}")]
        public async Task<ActionResult<List<SellerMonthlyRevenueDto>>> GetSellerMonthlyRevenue(int sellerId)
        {
           
            var result = await _transactionManager.GetSellerMonthlyRevenueAsync(sellerId);

            return Ok(result);
        }

        [HttpGet("Total-Transactions")]
        public async Task<IActionResult> GetTotal()
        {
            var result = await _transactionManager.GetTotalTransactionsCount();
            return Ok(result);
        }

        [HttpGet("successful")]
        public async Task<IActionResult> GetSuccessful()
        {
            var result = await _transactionManager.GetSuccessfulTransactionsCount();
            return Ok(result);
        }

        [HttpGet("failed")]
        public async Task<IActionResult> GetFailed()
        {
            var result = await _transactionManager.GetFailedTransactionsCount();
            return Ok(result);
        }
    }
}
