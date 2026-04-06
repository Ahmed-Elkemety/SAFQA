using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.PaymentDto
{
    public class AddCardDto
    {
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }  
        public string CVV { get; set; }
        public string CardholderName { get; set; }
        public string CardLabel { get; set; }    
    }
}
