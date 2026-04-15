using SAFQA.BLL.Dtos.UserAppDto.ChatDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.UserAppManager.ChatService
{
    public interface IChatService
    {
        ConversationDto GetOrCreateConversation(int disputeId, string userId);
        Task SendMessage(SendMessageDto dto);
    }
}
