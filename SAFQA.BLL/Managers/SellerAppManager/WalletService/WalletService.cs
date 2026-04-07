using SAFQA.BLL.Dtos.SellerAppDto.WalletDto;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.SellerDashboard.TransactionRepo;
using SAFQA.DAL.Repository.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.WalletService
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepo _walletRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly ICardRepo _cardRepo;
        public WalletService(IWalletRepo walletRepo, ITransactionRepository transactionRepo, ICardRepo cardRepo) 
        {
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _cardRepo = cardRepo;
        }

        public bool DepositMoney(string userId, DepositeMoneyDto dto, out string message)
        {
            message = "";

            var wallet = _walletRepo.GetByIdd(userId);
            if (wallet == null)
            {
                message = "Wallet not found";
                return false;
            }

            decimal balanceBefore = wallet.Balance;

            var card = _cardRepo.GetById(dto.SavedCardId.Value);

            if (dto.SavedCardId != null)
            {
                if (card == null || card.WalletId != wallet.Id)
                {

                    message = "Card not found or does not belong to your wallet";
                    return false;
                }
            }

            
            wallet.Balance += dto.Amount;
            wallet.UpdatedAt = DateTime.UtcNow;
            _walletRepo.Update(wallet);

            string description;

            if (dto.SavedCardId != null)
            {
                description = $"Deposit from card ending with {card.Last4Digits}";
            }
            else
            {
                description = "Cash deposit";
            }

            
            var transaction = new Transactions
            {
                WalletId = wallet.Id,
                Type = TransactionType.Deposit,
                Amount = dto.Amount,
                BalanceBefore = balanceBefore,
                BalanceAfter = wallet.Balance,
                Status = TransactionStatus.Completed,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };
            _transactionRepo.Add(transaction);

            message = "Deposit successful";
            return true;
        }

        public bool WithdrawMoney(string userId,WithdrawDto dto, out string message)
        {
            message = "";

            if (dto.Amount <= 0)
            {
                message = "Invalid amount";
                return false;
            }



            var wallet = _walletRepo.GetByIdd(userId);

            if (wallet == null)
            {
                message = "Wallet not found";
                return false;
            }

            if (wallet.Balance < dto.Amount)
            {
                message = "Insufficient balance";
                return false;
            }

            var card = _cardRepo.GetById(dto.CardId);

            var balanceBefore = wallet.Balance;
            wallet.Balance -= dto.Amount;
            wallet.UpdatedAt = DateTime.Now;
            _walletRepo.Update(wallet);


            string description = $"Withdraw to {card.CardBrand} ending with {card.Last4Digits}";

            var transaction = new Transactions
            {
                WalletId = wallet.Id,
                Amount = dto.Amount,
                BalanceBefore = balanceBefore,
                BalanceAfter = wallet.Balance,
                Type = TransactionType.Withdrawal,
                Status = TransactionStatus.Completed,
                Description = description,
                CreatedAt = DateTime.Now
            };

            _transactionRepo.Add(transaction);

            message = "Withdraw successful";
            return true;
        }

        public WalletBalanceDto GetBalance(string userId, out string message)
        {
            message = "";

            var wallet = _walletRepo.GetByIdd(userId);

            if (wallet == null)
            {
                message = "Wallet not found";
                return null;
            }

            return new WalletBalanceDto
            {
                WalletId = wallet.Id,
                Balance = wallet.Balance
            };
        }
        public IEnumerable<TransactionHistoryDto> GetTransactionHistory(string userId)
        {
            var wallet = _walletRepo.GetAll()
                           .FirstOrDefault(w => w.UserId == userId);

            if (wallet == null)
                return Enumerable.Empty<TransactionHistoryDto>();

            var transactions = _transactionRepo.GetAll()
                                                   .Where(t => t.WalletId == wallet.Id)
                                                   .OrderByDescending(t => t.CreatedAt)
                                                   .ToList();

            return transactions.Select(t => new TransactionHistoryDto
            {
                Date = t.CreatedAt,
                Type = t.Type.ToString(),
                Amount = t.Amount,
            }).ToList();
        }
    }
}
