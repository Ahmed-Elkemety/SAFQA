using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.AuctionDto.AuctionProcessDtos
{
    public class ItemAttributeDto
    {
        public int Id { get; set; }
        public string AttributeName { get; set; }
        public byte[] Image { get; set; }

        public string Value { get; set; }
    }
}
