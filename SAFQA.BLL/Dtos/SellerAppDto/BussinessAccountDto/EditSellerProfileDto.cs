using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.BussinessAccountDto
{
    public class EditSellerProfileDto
    {
        public string StoreName { get; set; }
        public string PhoneNumber { get; set; }
        public int CityId { get; set; }
        public string? Description { get; set; }
        public string? StoreLogo { get; set; }
    }
}
