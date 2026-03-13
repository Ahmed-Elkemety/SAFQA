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
        IQueryable<Transactions> GetPendingPayments(int sellerId);

        IQueryable<Transactions> GetSellerPayments(int sellerId);

        Task<decimal> GetTotalRevenueAsync(int sellerId);
    }
}
