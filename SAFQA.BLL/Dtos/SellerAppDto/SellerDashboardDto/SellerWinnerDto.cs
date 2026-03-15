using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto
{
    public class SellerWinnerDto
    {
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string SellerStoreName { get; set; }
        public int WonAuctionsCount { get; set; }
        public decimal TotalNetProfit { get; set; }
    }
}
