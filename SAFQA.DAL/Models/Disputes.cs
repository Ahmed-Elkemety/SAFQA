using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Enums;

namespace SAFQA.DAL.Models
{
    public class Disputes
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DisputeStatus Status { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
        public int? UserId { get; set; }
        public int? AuctionId { get; set; }
        public int? DeliveryId { get; set; }
        public Delivery Delivery { get; set; }
        public User User { get; set; }
        public Auction Auction { get; set; }
    }
}
