using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SAFQA.DAL.Repository.SellerDashboard.TransactionRepo
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly SAFQA_Context _context;
        public TransactionRepository(SAFQA_Context context) 
        {
            _context = context;   
        }

        public async Task<List<(int Month, decimal Revenue)>> GetSellerMonthlyRevenue(int sellerId)
        {
            return await _context.Transactions
            .Where(t => t.Status == Enums.TransactionStatus.Completed &&
                        t.Type == TransactionType.Deposit &&
                        t.Wallet.User.Seller.Id == sellerId)
            .GroupBy(t => t.CreatedAt.Month)
            .Select(g => new ValueTuple<int, decimal>(
                g.Key,
                g.Sum(t => t.Amount)
            ))
            .ToListAsync(); 
        }

        public async Task<int> GetTotalPendingPayments(int sellerId)
        {
            return await _context.Transactions
                    .Where(t => t.Status == Enums.TransactionStatus.Pending &&
                                t.Wallet != null &&
                                t.Wallet.User != null &&
                                t.Wallet.User.Seller != null &&
                                t.Wallet.User.Seller.Id == sellerId)
                    .CountAsync();
        }



        public async Task<decimal> GetTotalRevenueAsync(int sellerId)
        {
            var totalDeposits = await _context.Transactions
             .Where(t => t.Type == TransactionType.Deposit                 
                         && t.Status == Enums.TransactionStatus.Completed     
                         && t.Wallet != null
                         && t.Wallet.User != null
                         && t.Wallet.User.Seller != null
                         && t.Wallet.User.Seller.Id == sellerId)         
             .SumAsync(t => t.Amount);                                    

            return totalDeposits;
        }

        public async Task<int> GetTotalTransactionsCount()
        {
            return await _context.Transactions
                .CountAsync();
        }
        public async Task<int> GetSuccessfulTransactionsCount()
        {
            return await _context.Transactions
                .Where(t => t.Status == Enums.TransactionStatus.Completed)
                .CountAsync();
        }

        public async Task<int> GetFailedTransactionsCount()
        {
            return await _context.Transactions
                .Where(t => t.Status == Enums.TransactionStatus.Failed)
                .CountAsync();
        }

        public IQueryable<Transactions> GetAll()
        {
            return _context.Transactions;
        }

        public Transactions GetById(int id)
        {
            return _context.Transactions.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Transactions transaction)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }

        public void Update(Transactions transaction)
        {
            _context.Transactions.Update(transaction);
            _context.SaveChanges();
        }

        public void Delete(Transactions transaction)
        {
            _context.Transactions.Remove(transaction);
            _context.SaveChanges();
        }
    }
}