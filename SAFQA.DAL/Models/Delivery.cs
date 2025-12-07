using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Enums;

namespace SAFQA.DAL.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime ComfirmedAt { get; set; }
        public string ProofImgUrl { get; set; }
        public DeliveryStatus Status { get; set; }
        public int AuctionId { get; set; }
        public int SellerId { get; set; }
        public int UserId { get; set; }
        public ICollection<Disputes> Disputes { get; set; }
        public Auction Auction { get; set; }
        public User User { get; set; }
        public Seller Seller { get; set; } 
    }
}
