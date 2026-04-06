using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.WalletDto
{
    public class WithdrawDto
    {
        public decimal Amount { get; set; }
        public int CardId { get; set; }
    }
}
