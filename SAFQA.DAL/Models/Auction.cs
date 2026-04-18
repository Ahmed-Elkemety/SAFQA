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

        public int? SellerId { get; set; }
        public Seller Seller { get; set; }

        public string WinnerUserId { get; set; } // ✅ nullable

        public string Title { get; set; }
        public byte[]? Image { get; set; }
        public string Description { get; set; }

        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }

        public decimal FinalPrice { get; set; } // ✅ nullable
        public decimal SecurityDeposit { get; set; } // ✅ nullable

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int BidIncrement { get; set; }

        public AuctionStatus Status { get; set; } = AuctionStatus.Upcoming;

        public int LikesCount { get; set; }
        public int ParticipationCount { get; set; }
        public int ViewsCount { get; set; }
        public int TotalBids { get; set; }

        public bool IsFeatured { get; set; }
        public int? CountDown { get; set; }
        public bool IsTrending { get; set; }
        public bool HotScore { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
        public string? DeletedAt { get; set; } // ✅ بدل string

        // Navigation
        public Review? review { get; set; } // ✅ nullable
        public Delivery? delivery { get; set; } // ✅ nullable
        public ICollection<AuctionUser> AuctionUsers { get; set; }
        public ICollection<Bid> Bids { get; set; }
        public ICollection<ProxyBidding> ProxyBiddings { get; set; }
        public ICollection<Disputes> disputes { get; set; }
        public ICollection<Item> items { get; set; }
        public ICollection<AuctionReport> Reports { get; set; }
        public OrderTracking orderTracking { get; set; }
    }
}
