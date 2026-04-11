using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class AuctionReport
    {
        public int Id { get; set; }

        public int AuctionId { get; set; }
        public Auction Auction { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public string Reason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
