using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto
{
    public class CategoryStatsDto
    {
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public double Percentage { get; set; }
    }
}
