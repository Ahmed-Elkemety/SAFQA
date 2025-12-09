using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Enums;

namespace SAFQA.DAL.Models
{
    public class Bid
    {
        public int Id { get; set; }
        public BidType Type { get; set; }
        public decimal Amount { get; set; }
        public int BidOrder { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public int AuctionId { get; set; }
        public  User User { get; set; }
        public Auction Auction { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}