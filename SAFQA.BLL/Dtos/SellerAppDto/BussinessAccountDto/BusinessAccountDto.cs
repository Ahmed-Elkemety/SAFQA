using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.BussinessAccountDto
{
    public class BusinessAccountDto
    {
        public string StoreName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string City { get; set; }
        public string Country { get; set; }

        public float SellerRating { get; set; }
        public int Followers { get; set; }

        public int AuctionsCount { get; set; }

        public string UpgradeType { get; set; }

        public string? StoreLogo { get; set; }
    }
}
