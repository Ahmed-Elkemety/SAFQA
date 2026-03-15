using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SAFQA.BLL.Dtos.AccountDto.Seller
{
    public class BusinessSellerDto
    {
        public IFormFile CommercialRegister { get; set; }
        public IFormFile TaxId { get; set; }
        public IFormFile OwnerNationalIdFront { get; set; }
        public IFormFile OwnerNationalIdBack { get; set; }

        public int? instaPayNumber { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string IBAN { get; set; }
        public string? LocalAccountNumber { get; set; }

    }
}
