using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SAFQA.BLL.Enums;
using SAFQA.DAL.Enums;

namespace SAFQA.DAL.Models
{
    public class User:IdentityUser
    {
        public string FullName { get; set; }
        public byte[]? Image { get; set; }
        public DateOnly BirthDate { get; set; }
        public GenderType Gender { get; set; }
        public UserStatus Status { get; set; }
        public UserLanguage Language { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public int? CityId { get; set; }
        public City City { get; set; }

        public Seller Seller { get; set; }
        public Wallet Wallet { get; set; }

        public ICollection<AuctionUser> AuctionUsers { get; set; }
        public ICollection<Bid> Bids { get; set; }
        public ICollection<ProxyBidding> ProxyBiddings { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<Disputes> Disputes { get; set; }
        public ICollection<Delivery> Deliveries { get; set; }
        public ICollection<Review> Reviews { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
