using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public int DeliverySpeed { get; set; }
        public int accurateDescription { get; set; }

        public string? UserId { get; set; }
        public int? SellerId { get; set; }
        public int? AuctionId { get; set; }
        public Auction auction { get; set; }
        public User User { get; set; }
        public Seller Seller { get; set; }
    }
}
