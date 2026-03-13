using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.SellerDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.SellerDashboard
{
    public class TransactionManager : ITransactionManager
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionManager(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public IQueryable<Transactions> GetPendingPayments(int sellerId)
        {
            return (IQueryable<Transactions>)_transactionRepository.GetPendingPayments(sellerId);
        }

        public IQueryable<Transactions> GetSellerPayments(int sellerId)
        {
            return (IQueryable<Transactions>)_transactionRepository.GetSellerPayments(sellerId);
        }

        public async Task<decimal> GetTotalRevenueAsync(int sellerId)
        {
            return await _transactionRepository.GetTotalRevenueAsync(sellerId);
        }
    }
}
