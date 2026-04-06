using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SAFQA.BLL.Enums;

namespace SAFQA.BLL.Dtos.AccountDto.Seller
{
    public class CreateSellerDto
    {
        public string StoreName { get; set; }
        public string PhoneNumber { get; set; }
        public int CityId { get; set; }
        public SellerBusinessType BusinessType { get; set; }
        public IFormFile? Logo { get; set; } // بدل byte[]

        public string Description { get; set; }
    }
}
