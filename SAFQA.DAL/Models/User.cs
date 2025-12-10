using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Enums;

namespace SAFQA.DAL.Models
{
    public class User
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Gender { get; set; }
        public UserRole Role { get; set; } // User - Seller - Admin
        public UserStatus Status { get; set; } // Active - Inactive - Blocked - Deleted
        public UserLanguage language { get; set; } // Arabic - English
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public bool DeletedAt { get; set; }
        public DateTime LastLogin { get; set; }

        // Navigation
        public int? CityId { get; set; }
        public City City { get; set; }
        public Seller Seller { get; set; } // 1 to 1
        public Wallet wallet { get; set; } // 1 to 1
        public ICollection<Auction> Auctions { get; set; } // M to N
        public ICollection<Bid> Bids { get; set; } // 1 to M
        public ICollection<ProxyBidding> proxyBiddings { get; set; }
        public ICollection<Notification> notifications { get; set; } // 1 to M
        public ICollection<Disputes> disputes { get; set; } // 1 to M
        public ICollection<Delivery> deliveries { get; set; } // 1 to M
        public ICollection<Review> reviews { get; set; } // 1 to M
    }
}
