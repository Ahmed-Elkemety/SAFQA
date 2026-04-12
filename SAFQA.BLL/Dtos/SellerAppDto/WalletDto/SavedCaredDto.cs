using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.SellerAppDto.WalletDto
{
    public class SavedCaredDto
    {
        public int CardId { get; set; }
        public string MaskedCardNumber { get; set; }
        public string cardlabel { get; set; }
    }
}