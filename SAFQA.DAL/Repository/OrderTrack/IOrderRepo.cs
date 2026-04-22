using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.OrderTrack
{
    public interface IOrderRepo
    {
        Task<List<OrderTracking>> GetAllAsync();
        Task<OrderTracking?> GetByIdAsync(int id);
        Task AddAsync(OrderTracking order);
        Task UpdateAsync(OrderTracking order);
        Task DeleteAsync(OrderTracking order);
    }
}