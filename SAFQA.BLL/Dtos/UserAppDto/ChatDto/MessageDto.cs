using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.ChatDto
{
    public class MessageDto
    {
        public int MessageId { get; set; }

        public string SenderName { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsSeen { get; set; }

        public List<string> Attachments { get; set; }
    }
}
