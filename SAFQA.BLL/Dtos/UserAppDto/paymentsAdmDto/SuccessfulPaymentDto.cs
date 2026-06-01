using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.paymentsAdmDto
{
    public class SuccessfulPaymentDto
    {
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public DateTime Date { get; set; }
    }
}
