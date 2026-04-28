using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.AuctionDto
{
    public class ItemAttributeDto
    {
        public int CategoryAttributeId { get; set; }

        public byte[]? Image { get; set; }

        public string Value { get; set; }
    }
}
