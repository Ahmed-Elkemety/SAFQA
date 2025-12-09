using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Enums;

namespace SAFQA.DAL.Models
{
    public class Auction
    {
        public int Id { get; set; }

        // FK to Seller (1-to-1)
        public int SellerId { get; set; }
        public Seller Seller { get; set; }
        public int WinnerUserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal SecurityDeposit { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public AuctionStatus Status { get; set; } // Upcoming - Active - EndingSoon - Finished - Cancelled

        public int LikesCount { get; set; }
        public int ParticipationCount { get; set; }
        public int ViewsCount { get; set; }
        public int TotalBids { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsTrending { get; set; }
        public bool HotScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }



        // Navigation
        public Delivery delivery { get; set; } // 1 to 1
        public ICollection<User> users { get; set; } // M to N
        public ICollection<Notification> notifications { get; set; } // 1 to M
        public ICollection<Bid> Bids { get; set; } // 1 to M
        public ICollection<Disputes> disputes { get; set; } // 1 to M
        public ICollection<Item> items { get; set; } // 1 to M
    }
}
