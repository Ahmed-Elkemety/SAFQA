using SAFQA.BLL.Dtos.UserAppDto.OrdersDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.UserAppManager.OrderService
{
    public interface IOrderService
    {
        Task<List<UserOrderDto>> GetUserDeliveredOrders(string userId);
        Task<List<UserInProgressOrderDto>> GetUserInProgressOrders(string userId);
    }
}
