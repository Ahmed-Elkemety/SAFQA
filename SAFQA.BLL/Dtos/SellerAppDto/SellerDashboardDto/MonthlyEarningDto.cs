using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto
{
    public class MonthlyEarningDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalEarnings { get; set; }
        public int AuctionCount { get; set; }
    }
}
