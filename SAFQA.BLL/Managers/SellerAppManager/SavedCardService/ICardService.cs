using SAFQA.BLL.Dtos.SellerAppDto.PaymentDto;
using SAFQA.BLL.Dtos.SellerAppDto.WalletDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.WalletServeice
{
    public interface ICardService
    {
        bool AddCard(string userId, AddCardDto dto, out string message);
        IEnumerable<SavedCaredDto> GetCardsByUser(string userId);
    }
}
