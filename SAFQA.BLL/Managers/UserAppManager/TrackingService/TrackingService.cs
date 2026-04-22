using SAFQA.BLL.Dtos.UserAppDto.TrackingDto;
using SAFQA.DAL.Repository.Auction;
using SAFQA.DAL.Repository.Delivery;
using SAFQA.DAL.Repository.OrderTrack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.UserAppManager.TrackingService
{
    public class TrackingService : ITrackingService
    {
        private readonly IAuctionRepository _auctionRepo;
        private readonly IDeliveryRepo _deliveryRepo;
        private readonly IOrderRepo _trackingRepo;

        public TrackingService(
            IAuctionRepository auctionRepo,
            IDeliveryRepo deliveryRepo,
            IOrderRepo trackingRepo)
        {
            _auctionRepo = auctionRepo;
            _deliveryRepo = deliveryRepo;
            _trackingRepo = trackingRepo;
        }

        public async Task<OrderTrackingDto> GetOrderTrackingAsync(int auctionId)
        {
            var auction = _auctionRepo.GetById(auctionId);

            if (auction == null)
                return null;

            var delivery = await _deliveryRepo.GetByAuctionIdAsync(auctionId);

            var trackingSteps = await _trackingRepo.GetAllAsync();

            var steps = trackingSteps
                .Where(t => t.AuctionId == auctionId)
                .OrderBy(t => t.Date)
                .Select(t => new OrderStatusStepDto
                {
                    StepName = t.Step.ToString(),
                    Date = t.Date,
                    IsCompleted = t.IsCompleted
                })
                .ToList();
            return new OrderTrackingDto
            {
                AuctionId = auction.Id,

                ProductTitle = auction.Title,
                ProductImage = auction.Image,

                TrackingId = delivery?.Code,

                DeliveryDate = delivery?.ComfirmedAt ?? auction.EndDate,

                StatusSteps = steps
            };
        }
    }
}