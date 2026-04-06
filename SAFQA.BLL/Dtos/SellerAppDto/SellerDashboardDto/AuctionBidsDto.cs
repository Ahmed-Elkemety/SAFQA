using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto
{
    public class AuctionBidsDto
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public int TotalBids { get; set; }
    }
}