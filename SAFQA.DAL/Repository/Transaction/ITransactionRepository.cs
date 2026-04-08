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
    }
}
