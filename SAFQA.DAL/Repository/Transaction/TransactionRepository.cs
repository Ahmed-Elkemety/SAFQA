using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Repository.Transaction
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly SAFQA_Context _context;

        public TransactionRepository(SAFQA_Context context)
        {
            _context = context;
        }

        public async Task AddAsync(Transactions transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }
    }
}
