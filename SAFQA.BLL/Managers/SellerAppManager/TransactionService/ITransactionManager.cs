using SAFQA.BLL.Dtos.SellerAppDto.SellerDashboardDto;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.TransactionService
{
    public interface ITransactionManager
    {
        Task<int> GetTotalPendingPayments(int sellerId);

        Task<decimal> GetTotalRevenueAsync(int sellerId);
        Task<List<SellerMonthlyRevenueDto>> GetSellerMonthlyRevenueAsync(int sellerId);
        Task<int> GetTotalTransactionsCount();
        Task<int> GetSuccessfulTransactionsCount();
        Task<int> GetFailedTransactionsCount();

    }
}