using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Repository.Transaction
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transactions transaction);
        Task<int> GetTotalPendingPayments(int sellerId);
        Task<decimal> GetTotalRevenueAsync(int sellerId);
        Task<List<(int Month, decimal Revenue)>> GetSellerMonthlyRevenue(int sellerId);
        Task<int> GetTotalTransactionsCount();
        Task<int> GetSuccessfulTransactionsCount();
        Task<int> GetFailedTransactionsCount();

        IQueryable<Transactions> GetAll();
        Transactions GetById(int id);
        void Add(Transactions card);
        void Update(Transactions card);
        void Delete(Transactions card);
    }
}
