using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SAFQA.BLL.Dtos.SellerAppDto.AuctionDto
{
    public class ViewAuctionDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[]? Image { get; set; }

        public decimal StartingPrice { get; set; }

        public int BidIncrement { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<ViewItemDto> Items { get; set; }
    }
}
