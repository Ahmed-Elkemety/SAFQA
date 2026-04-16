using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Enums;

namespace SAFQA.BLL.Dtos.SellerAppDto.AuctionDto
{
    public class SellerActionHistoryDto
    {
        public int AuctionId { get; set; }
        public string Title { get; set; }
        public decimal DisplayPrice { get; set; }
        public DateTime DisplayDate { get; set; }
        public int TotalBids { get; set; }

        public AuctionStatus Status { get; set; }

        public byte[]? Image { get; set; }
    }
}
