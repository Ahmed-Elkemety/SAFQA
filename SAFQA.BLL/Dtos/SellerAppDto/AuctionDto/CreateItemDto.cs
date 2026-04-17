using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SAFQA.DAL.Enums;

namespace SAFQA.BLL.Dtos.SellerAppDto.AuctionDto
{
    public class CreateItemDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public ItemCondition Condition { get; set; }
        public string WarrantyInfo { get; set; }

        public List<ItemAttributeDto> Attributes { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
