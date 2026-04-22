using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.OrderTrack
{
    public class OrderRepo : IOrderRepo
    {
        private readonly SAFQA_Context _context;
        public OrderRepo(SAFQA_Context context)
        {
            _context = context;
        }
        public async Task<List<OrderTracking>> GetAllAsync()
        {
            return await _context.OrderTrackings.ToListAsync();
        }

        public async Task<OrderTracking> GetByIdAsync(int id)
        {
            return await _context.OrderTrackings
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(OrderTracking orderTracking)
        {
            await _context.OrderTrackings.AddAsync(orderTracking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrderTracking orderTracking)
        {
            _context.OrderTrackings.Update(orderTracking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(OrderTracking orderTracking)
        {
            _context.OrderTrackings.Remove(orderTracking);
            await _context.SaveChangesAsync();
        }
    }
}
