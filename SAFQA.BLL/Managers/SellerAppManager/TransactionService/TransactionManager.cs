using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.TransactionService
{
    public class TransactionManager : ITransactionManager
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionManager(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public Task<int> GetTotalPendingPayments(string userId)
        {
            return _transactionRepository.GetTotalPendingPayments(userId);
        }

        public async Task<decimal> GetTotalRevenueAsync(string userId)
        {
            return await _transactionRepository.GetTotalRevenueAsync(userId);
        }

        public async Task<List<SellerMonthlyRevenueDto>> GetSellerMonthlyRevenueAsync(string userId)
        {
            var revenueData = await _transactionRepository
                .GetSellerMonthlyRevenue(userId);

            return revenueData
                .Select(r => new SellerMonthlyRevenueDto
                {
                    Month = r.Month,
                    Revenue = r.Revenue
                })
                .ToList();
        }

        public async Task<int> GetTotalTransactionsCount()
        {
            return await _transactionRepository.GetTotalTransactionsCount();
        }

        public async Task<int> GetSuccessfulTransactionsCount()
        {
            return await _transactionRepository.GetSuccessfulTransactionsCount();
        }

        public async Task<int> GetFailedTransactionsCount()
        {
            return await _transactionRepository.GetFailedTransactionsCount();
        }
    }
}