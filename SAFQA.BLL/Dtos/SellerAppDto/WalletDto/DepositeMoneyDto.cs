using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.WalletDto
{
    public class DepositeMoneyDto
    {
        public decimal Amount { get; set; }
        public int? SavedCardId { get; set; }
    }
}