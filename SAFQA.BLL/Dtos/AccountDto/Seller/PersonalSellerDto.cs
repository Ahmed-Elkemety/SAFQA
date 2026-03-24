using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SAFQA.BLL.Dtos.AccountDto.Seller
{
    public class PersonalSellerDto
    {
        public IFormFile NationalIdFront { get; set; }
        public IFormFile NationalIdBack { get; set; }
        public IFormFile SelfieWithId { get; set; }
    }
}
