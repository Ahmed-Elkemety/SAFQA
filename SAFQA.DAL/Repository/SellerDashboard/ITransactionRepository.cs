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
        Task<int> GetTotalPendingPayments(int sellerId);
        Task<decimal> GetTotalRevenueAsync(int sellerId);
        Task<List<(int Month, decimal Revenue)>> GetSellerMonthlyRevenue(int sellerId);

    }
}
