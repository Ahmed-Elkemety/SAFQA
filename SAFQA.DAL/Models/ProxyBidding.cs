using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Enums;

namespace SAFQA.DAL.Models
{
    public class ProxyBidding
    {
        public int Id { get; set; }
        public ProxyStatus Status { get; set; }
        public int Max { get; set; }
        public int Step { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public int? UserId { get; set; }
        public User user { get; set; }
        public int? AuctionId { get; set; }
        public Auction auction { get; set; }
        public ICollection<Bid> bids { get; set; }
    }
}
