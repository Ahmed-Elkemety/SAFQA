using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Wallet
{
    public class WalletRepo : IWalletRepo
    {
        private readonly SAFQA_Context _context;
        public WalletRepo(SAFQA_Context context)
        {
            _context = context;
        }

        public IQueryable<Models.Wallet> GetAll()
        {
            return _context.Wallets;
        }

        public Models.Wallet GetById(int id)
        {
            return _context.Wallets.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Models.Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
        }

        public void Update(Models.Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }
        public void Delete(Models.Wallet wallet)
        {
            _context.Wallets.Remove(wallet);
            _context.SaveChanges();
        }

        public Models.Wallet GetByIdd(string userId)
        {
            return _context.Wallets.FirstOrDefault(w => w.UserId == userId);
        }
    }
}