using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Repository.OrderTracking
{
    public interface IOrderTracking
    {
        IQueryable<OrderTracking> GetAll();
        OrderTracking GetById(int id);
        OrderTracking GetByIdd(string userId);
        void Add(OrderTracking orderTracking);
        void Update(OrderTracking orderTracking);
        void Delete(OrderTracking orderTracking);
    }
}
