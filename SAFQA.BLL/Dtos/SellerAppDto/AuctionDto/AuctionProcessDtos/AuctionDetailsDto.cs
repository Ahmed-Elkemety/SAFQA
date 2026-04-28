using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.AuctionDto.AuctionProcessDtos
{
    public class AuctionDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal CurrentPrice { get; set; }
        public int TotalBids { get; set; }
        public decimal SecurityDeposit { get; set; }
        public int BidIncrement { get; set; }

        public int SellerId { get; set; }
        public string StoreName { get; set; }
        public string? StoreLogo { get; set; }

        public List<ItemDto> Items { get; set; }
    }
}
