using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SAFQA.BLL.Dtos.SellerAppDto.AuctionDto
{
    public class EditAuctionDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
        public List<EditItemDto> Items { get; set; }
    }
}
