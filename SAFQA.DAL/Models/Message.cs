using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class Message
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        public string SenderId { get; set; }
        public User Sender { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsSeen { get; set; }
        public DateTime? SeenAt { get; set; }

        public ICollection<MessageAttachment>? Attachments { get; set; }
    }
}
