using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.ChatDto
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public string SellerId { get; set; }
        public int DisputeId { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? LastMessage { get; set; }
    }
}
