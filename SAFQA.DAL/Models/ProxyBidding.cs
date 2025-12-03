using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Enums;

namespace SAFQA.DAL.Models
{
    public class ProxyBidding
    {
        public int BidId { get; set; }
        public int ProxyId { get; set; }
        public ProxyStatus Status { get; set; }
        public int Max { get; set; }
        public int Step { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
