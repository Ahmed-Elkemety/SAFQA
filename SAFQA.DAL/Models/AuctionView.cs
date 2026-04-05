using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class AuctionView
    {
        public int Id { get; set; }

        // مَن شاهد؟
        public string UserId { get; set; }
        public User User { get; set; }

        // ماذا شاهد؟
        public int AuctionId { get; set; }
        public Auction Auction { get; set; }

        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

        // اختياري: إذا أردت معرفة هل شاهد المزاد من الموبايل أم الكمبيوتر
        public string DeviceType { get; set; }
    }
}
