using SAFQA.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class OrderTracking
    {
        public int Id { get; set; }

        public int AuctionId { get; set; }
        public Auction Auction { get; set; }

        public TrackingStep Step { get; set; } 

        public DateTime Date { get; set; }

        public bool IsCompleted { get; set; }  
    }
}
