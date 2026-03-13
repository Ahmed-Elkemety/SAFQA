using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Repository.SellerDashboard
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly SAFQA_Context _context;
        public TransactionRepository(SAFQA_Context context) 
        {
            _context = context;   
        }

        public IQueryable<Transactions> GetPendingPayments(int sellerId)
        {

            var pendingPayments = from t in _context.Transactions
                                  join w in _context.Wallets on t.WalletId equals w.Id
                                  join u in _context.Users on w.UserId equals u.Id
                                  join s in _context.Sellers on u.Seller.Id equals s.Id
                                  where s.Id == sellerId && t.Status == Enums.TransactionStatus.Pending
                                  select t;

            return pendingPayments;
        }


        public IQueryable<Transactions> GetSellerPayments(int sellerId)
        {
            var sellerPayments = _context.Transactions
                                        .Where(t => t.Wallet != null &&
                                        t.Wallet.User != null &&
                                        t.Wallet.User.Seller != null &&
                                        t.Wallet.User.Seller.Id == sellerId);

            return sellerPayments;
        }


        public async Task<decimal> GetTotalRevenueAsync(int sellerId)
        {
            var totalRevenue = await _context.Transactions
                    .Where(t => t.Wallet != null &&
                                t.Wallet.User != null &&
                                t.Wallet.User.Seller != null &&
                                t.Wallet.User.Seller.Id == sellerId)
                    .SumAsync(t => t.Amount); 

            return totalRevenue;
        }
    }
}
