using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAFQA.BLL.Dtos.UserAppDto.ChatDto;
using SAFQA.BLL.Managers.SellerAppManager.WalletServeice;
using SAFQA.BLL.Managers.UserAppManager.ChatService;
using SAFQA.BLL.Managers.UserAppManager.ConversationService;
using System.Security.Claims;

namespace SAFQA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [Authorize(Roles = "USER")]
        [HttpPost("conversation/{disputeId}")]
        public IActionResult GetOrCreateConversation(int disputeId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return Unauthorized();
                var conversation = _chatService.GetOrCreateConversation(disputeId, userId);
                return Ok(conversation);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize(Roles = "USER")]
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            try
            {
                await _chatService.SendMessage(dto);
                return Ok(new { message = "Message sent successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}