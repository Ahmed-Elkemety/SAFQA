using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.AuctionDto.AuctionProcessDtos
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public string Condition { get; set; }
        public string WarrantyInfo { get; set; }

        public List<string> Images { get; set; }
        public List<ItemAttributeDto> Attributes { get; set; }
    }
}
