using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.Delivery
{
    public interface IDeliveryRepo
    {
        Task<List<Models.Delivery>> GetAllAsync();
        Task<Models.Delivery?> GetByIdAsync(int id);
        Task AddAsync(Models.Delivery delivery);
        Task UpdateAsync(Models.Delivery delivery);
        Task DeleteAsync(Models.Delivery delivery);
    }
}
