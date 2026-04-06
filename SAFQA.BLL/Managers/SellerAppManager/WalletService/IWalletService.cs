using SAFQA.BLL.Dtos.SellerAppDto.WalletDto;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.WalletService
{
    public interface IWalletService
    {
        bool DepositMoney(string userId, DepositeMoneyDto dto, out string message);
        bool WithdrawMoney(string userId, WithdrawDto dto, out string message);
        WalletBalanceDto GetBalance(string userId, out string message);
        IEnumerable<TransactionHistoryDto> GetTransactionHistory(string userId);
    }
}