using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class AuctionUser
    {
        public int? AuctionId { get; set; }
        public Auction Auction { get; set; }

        public string? UserId { get; set; }
        public User User { get; set; }

        public DateTime JoinedAt { get; set; }
    }
}
