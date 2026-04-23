using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Wallet
{
    public interface IWalletRepo
    {
        IQueryable<Models.Wallet> GetAll();
        Models.Wallet GetById(int id);
        Models.Wallet GetByIdd(string userId);
        void Add(Models.Wallet wallet);
        void Update(Models.Wallet wallet);
        void Delete(Models.Wallet wallet);
        Task<Models.Wallet?> GetByUserIdAsync(string userId);

    }
}