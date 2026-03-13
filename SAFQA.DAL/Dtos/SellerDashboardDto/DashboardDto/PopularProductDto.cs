using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Dtos.SellerDashboardDto.DashboardDto
{
    public class PopularProductDto
    {
        public string ProductName { get; set; }
        public int TotalBids { get; set; }
        public int TotalAuctions { get; set; }
    }
}
