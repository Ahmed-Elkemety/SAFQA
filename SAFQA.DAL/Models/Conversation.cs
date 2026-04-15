using SAFQA.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class Conversation
    {
        public int Id { get; set; }

        public string BuyerId { get; set; }
        public User Buyer { get; set; }

        public string SellerUserId { get; set; }
        public User SellerUser { get; set; }

        public int DisputeId { get; set; }
        public Disputes Dispute { get; set; }

        public ConversationType Type { get; set; } = ConversationType.Dispute;

        public DateTime CreatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public string? LastMessage { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}
