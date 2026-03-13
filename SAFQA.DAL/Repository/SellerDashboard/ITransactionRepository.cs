using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SAFQA.DAL.Repository.SellerDashboard
{
    public interface ITransactionRepository
    {
        IQueryable<Transactions> GetSellerPayments(int sellerId);
        IQueryable<Transactions> GetPendingPayments(int sellerId);
        Task<decimal> GetTotalRevenueAsync(int sellerId);
    }
}
