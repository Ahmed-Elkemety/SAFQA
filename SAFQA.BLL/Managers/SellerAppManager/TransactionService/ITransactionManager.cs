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
        Task<int> GetTotalPendingPayments(string userId);

        Task<decimal> GetTotalRevenueAsync(string userId);
        Task<List<SellerMonthlyRevenueDto>> GetSellerMonthlyRevenueAsync(string userId);
        Task<int> GetTotalTransactionsCount();
        Task<int> GetSuccessfulTransactionsCount();
        Task<int> GetFailedTransactionsCount();

    }
}