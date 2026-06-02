using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.BLL.Managers.AdminService;
using SAFQA.BLL.Managers.SellerAppManager.TransactionService;
using System.Security.Claims;

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


        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingPayments()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var pendingPayments = await _transactionManager.GetTotalPendingPayments(userId);

            return Ok(pendingPayments);
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var totalRevenue = await _transactionManager.GetTotalRevenueAsync(userId);

            return Ok(totalRevenue);
        }

        [HttpGet("MonthlyRevenue")]
        public async Task<ActionResult<List<SellerMonthlyRevenueDto>>> GetSellerMonthlyRevenue()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var result = await _transactionManager.GetSellerMonthlyRevenueAsync(userId);

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