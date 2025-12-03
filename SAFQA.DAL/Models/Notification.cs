using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Enums;

namespace SAFQA.DAL.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public NotificationTypes notification { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public decimal? BalanceAfter { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Descriptor { get; set; }
        public int UserId { get; set; }
        public Auction Auction { get; set; }
        public int BidsId { get; set; }
        public Bid Bids { get; set; }
        public User User { get; set; }
        public Transactions Transaction { get; set; }
    }
}
