using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Delivery
{
    public class DeliveryRepo
    {
        private readonly SAFQA_Context _context;
        public DeliveryRepo(SAFQA_Context context)
        {
            _context = context;
        }
        public async Task<List<Models.Delivery>> GetAllAsync()
        {
            return await _context.deliveries.ToListAsync();
        }

        public async Task<Models.Delivery> GetByIdAsync(int id)
        {
            return await _context.deliveries
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Models.Delivery delivery)
        {
            await _context.deliveries.AddAsync(delivery);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Models.Delivery delivery)
        {
            _context.deliveries.Update(delivery);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Models.Delivery delivery)
        {
            _context.deliveries.Remove(delivery);
            await _context.SaveChangesAsync();
        }
    }
}
