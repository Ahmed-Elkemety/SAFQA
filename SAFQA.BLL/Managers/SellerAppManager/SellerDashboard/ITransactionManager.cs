using SAFQA.BLL.Managers.Dtos;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.SellerAppManager.SellerDashboard
{
    public interface ITransactionManager
    {
        Task<int> GetTotalPendingPayments(int sellerId);

        Task<decimal> GetTotalRevenueAsync(int sellerId);
        Task<List<SellerMonthlyRevenueDto>> GetSellerMonthlyRevenueAsync(int sellerId);

    }
}