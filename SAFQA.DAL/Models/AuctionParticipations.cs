using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Models
{
    public class AuctionParticipations
    {
        public int Id { get; set; }

        public int AuctionId { get; set; }
        public virtual Auction Auction { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime PatoicipationTime { get; set; }
    }
}
