using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.BLL.Managers.AdminService;
using SAFQA.BLL.Managers.SellerAppManager.TransactionService;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IAdminService _adminservice;

        public TransactionController(ITransactionManager transactionManager, IAdminService adminservice)
        {
            _transactionManager = transactionManager;
            _adminservice = adminservice;
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

        [HttpGet("successful/Payments/Table")]
        public IActionResult GetSuccessfulPayments([FromQuery] int days = 7)
        {
            var result = _adminservice.GetSuccessfulPayments(days);
            return Ok(result);
        }

        [HttpGet("failed/Payments/Table")]
        public IActionResult GetFailedPayments([FromQuery] int days = 7)
        {
            if (days <= 0)
                return BadRequest("Days must be greater than 0");

            var result = _adminservice.GetFailedPayments(days);

            return Ok(result);
        }

        [HttpPost("full-refund/{disputeId}")]
        public IActionResult FullRefund(int disputeId)
        {
            try
            {
                var result = _adminservice.FullRefund(disputeId);

                return Ok(new
                {
                    success = result.Success,
                    message = result.Message,
                    refundedAmount = result.RefundedAmount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("partial-refund")]
        public IActionResult PartialRefund(int DisputeId, decimal RefundAmount)
        {
            try
            {
                var result = _adminservice.PartialRefund(DisputeId, RefundAmount);

                return Ok(new
                {
                    success = result.Success,
                    message = result.Message,
                    refundedAmount = result.RefundedAmount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}