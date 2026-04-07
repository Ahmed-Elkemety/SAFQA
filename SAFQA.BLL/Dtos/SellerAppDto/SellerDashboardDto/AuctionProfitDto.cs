using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto
{
    public class AuctionProfitDto
    {
        public string Title { get; set; }      
        public decimal StartingPrice { get; set; }  
        public decimal FinalPrice { get; set; }     
        public decimal Profit { get; set; }         
        public string WinnerName { get; set; }
    }
}