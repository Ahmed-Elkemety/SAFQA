using SAFQA.DAL.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.OrderTracking
{
    public class OrderTracking : IOrderTracking
    {
        private readonly SAFQA_Context _context;
        public OrderTracking(SAFQA_Context context)
        {
            _context = context;
        }

        public IQueryable<OrderTracking> GetAll()
        {
            return _context.OrderTracking;
        }

        public Models.Wallet GetById(int id)
        {
            return _context.OrderTracking.FirstOrDefault(c => c.Id == id);
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
