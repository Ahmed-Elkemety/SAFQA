using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Dtos.UserAppDto.OrdersDto;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Repository.Auction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.UserAppManager.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IAuctionRepository _auctionRepo;
        public OrderService(IAuctionRepository auctionRepo) 
        {
            _auctionRepo = auctionRepo;
        }
        public async Task<List<UserOrderDto>> GetUserDeliveredOrders(string userId)
        {
            var orders = await _auctionRepo.GetAll()
                .Where(a => a.WinnerUserId == userId
                            && a.delivery != null
                            && a.delivery.Status == DeliveryStatus.Deliverd)

                .Select(a => new UserOrderDto
                {
                    AuctionId = a.Id,

                    Images = a.items
                        .SelectMany(i => i.images)
                        .Select(img => img.Image)
                        .ToList()
                })
                .ToListAsync();

            return orders;
        }

        public async Task<List<UserInProgressOrderDto>> GetUserInProgressOrders(string userId)
        {
            var orders = await _auctionRepo.GetAll()
                .Where(a => a.WinnerUserId == userId
                            && a.delivery != null
                            && a.delivery.Status != DeliveryStatus.Deliverd)

                .Select(a => new UserInProgressOrderDto
                {
                    AuctionId = a.Id,

                    ExpectedDeliveryDate = a.delivery.ComfirmedAt, 

                    Images = a.items
                        .SelectMany(i => i.images)
                        .Select(img => img.Image)
                        .ToList()
                })
                .ToListAsync();

            return orders;
        }

           }
}
