using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.SellerDashboard.TransactionRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.SellerDashboard.TransactionService
{
    public class TransactionManager : ITransactionManager
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionManager(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public Task<int> GetTotalPendingPayments(int sellerId)
        {
            return _transactionRepository.GetTotalPendingPayments(sellerId);
        }

        public async Task<decimal> GetTotalRevenueAsync(int sellerId)
        {
            return await _transactionRepository.GetTotalRevenueAsync(sellerId);
        }

         public async Task<List<SellerMonthlyRevenueDto>> GetSellerMonthlyRevenueAsync(int sellerId)
         {
            var revenueData = await _transactionRepository.GetSellerMonthlyRevenue(sellerId);

            return revenueData
                .Select(r => new SellerMonthlyRevenueDto
                {
                    Month = r.Month,
                    Revenue = r.Revenue
                })
                .ToList();
         }
    }
}