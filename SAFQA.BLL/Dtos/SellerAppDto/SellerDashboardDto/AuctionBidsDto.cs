using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto
{
    public class AuctionBidsDto
    {
        public int AuctionId { get; set; }
        public string AuctionTitle { get; set; }
        public List<string> ProductNames { get; set; }
        public int TotalBids { get; set; }
    }
}
