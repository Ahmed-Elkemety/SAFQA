using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SAFQA.BLL.Dtos.DeliveryDto
{
    public class Step4Dto
    {
        public int AuctionId { get; set; }
        public IFormFile Image { get; set; }
    }
}
