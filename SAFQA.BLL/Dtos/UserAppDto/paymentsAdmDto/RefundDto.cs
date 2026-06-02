using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.paymentsAdmDto
{
    public class RefundDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public decimal RefundedAmount { get; set; }
    }
}
