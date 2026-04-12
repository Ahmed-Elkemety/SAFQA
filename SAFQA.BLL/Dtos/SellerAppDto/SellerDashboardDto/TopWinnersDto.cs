using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto
{
    public class TopWinnerDto
    {
        public string BuyerName { get; set; }
        public string SellerCompanyName { get; set; }
        public string Email { get; set; }
        public int AuctionsWonCount { get; set; }
        public decimal TotalPaidAmount { get; set; }
    }
}