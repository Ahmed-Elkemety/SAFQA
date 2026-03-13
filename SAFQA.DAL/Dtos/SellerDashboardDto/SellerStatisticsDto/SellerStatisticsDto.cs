using SAFQA.DAL.Dtos.SellerDashboardDto.DashboardDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Dtos.SellerDashboardDto.SellerStatisticsDto
{
    public  class SellerStatisticsDto
    {
        public int TotalActiveAuctions { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalBids { get; set; }
        public int PendingPayments { get; set; }

        public List<MonthlyRevenueDto> RevenuePerMonth { get; set; } 
        public List<CategoryCountDto> ProductCategories { get; set; }
        public List<CategoryBidDto> BidsPerCategory { get; set; }
        public List<PopularProductDto> MostPopularProducts { get; set; }

        public List<string> SmartRecommendations { get; set; }
    }
}
