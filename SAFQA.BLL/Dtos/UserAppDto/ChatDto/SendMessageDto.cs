using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.ChatDto
{
    public class SendMessageDto
    {
        public int ConversationId { get; set; }
        public string SenderId { get; set; }
        public string Content { get; set; }
    }
}