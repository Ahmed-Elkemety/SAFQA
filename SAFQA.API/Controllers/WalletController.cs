using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.SellerAppDto.WalletDto;
using SAFQA.BLL.Managers.SellerAppManager.WalletService;
using System.Security.Claims;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [Authorize]
        [HttpPost("deposit")]
        public IActionResult Deposit([FromBody] DepositeMoneyDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            if (dto == null)
            {
                return BadRequest("Invalid data");
            }

            if (dto.Amount <= 0)
            {
                return BadRequest("Amount must be greater than 0");
            }

            bool result = _walletService.DepositMoney(userId,dto, out string message);

            if (!result)
            {
                return BadRequest(message);
            }

            return Ok(new
            {
                Message = message
            });
        }

        [Authorize]
        [HttpPost("withdraw")]
        public IActionResult Withdraw([FromBody] WithdrawDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            if (dto == null)
            {
                return BadRequest(new { message = "Invalid request" });
            }

            bool success = _walletService.WithdrawMoney(userId,dto, out string message);

            if (success)
            {
                return Ok(new { message = message });
            }
            else
            {
                return BadRequest(new { message = message });
            }
        }

        [Authorize]
        [HttpGet("balance")]
        public IActionResult GetBalance()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var balanceDto = _walletService.GetBalance(userId, out string message);


            if (balanceDto == null)  
                return NotFound(new { message });

            return Ok(balanceDto);
        }

        [Authorize]
        [HttpGet("TransactionHistory")]
        public IActionResult GetTransactionHistory()
        {
            // ClaimTypes.NameIdentifier ---> Get UserId
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var transactions = _walletService.GetTransactionHistory(userId);

            if (transactions == null || !transactions.Any())
                return NotFound(new { Message = "No transactions found for this wallet." });

            return Ok(transactions);
        }
    }
}