using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Enums;

namespace SAFQA.DAL.Models
{
    public class Seller
    {
        public int Id { get; set; }

        // FK to User (1-to-1)
        public int? UserId { get; set; }
        public User User { get; set; }

        public byte[] StoreLogo { get; set; }
        public string StoreName { get; set; }
        public string PhoneNumber { get; set; }
        public SellerBusinessType BussinessType { get; set; } // Individual - Company - Government - NonProfit
        public string Description { get; set; }
        public int Rating { get; set; }
        public string CommercialRegister { get; set; }
        public byte[] CommercialRegisterImage { get; set; }
        public SellerVerificationStatus VerificationStatus { get; set; } //  Pending - Verified - Rejected
        public StoreStatus StoreStatus { get; set; } // Active - Inactive - Suspended - Closed
        public DateTime SellerAt { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedAt { get; set; }

        // Navigation
        public ICollection<Auction> Auctions { get; set; } // 1 to M
        public ICollection<Review> reviews { get; set; } // 1 to M
        public ICollection<Delivery> deliveries { get; set; } // 1 to M
    }
}
