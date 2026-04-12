using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.SellerAppDto.PaymentDto;
using SAFQA.BLL.Managers.SellerAppManager.WalletServeice;
using System.Security.Claims;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [Authorize(Roles = "USER")]
        [HttpPost("AddCard")]
        public IActionResult AddCard([FromBody] AddCardDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();


            bool result = _cardService.AddCard(userId, dto, out string message);

            if (!result)
                return BadRequest(new { message });

            return Ok(new
            {
                message = message
            });
        }

        [HttpGet("cards")]
        public IActionResult GetUserCards()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var cards = _cardService.GetCardsByUser(userId);

            return Ok(cards);
        }

        [HttpDelete("Delete-Card/{id}")]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _cardService.DeleteCardAsync(id, userId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
