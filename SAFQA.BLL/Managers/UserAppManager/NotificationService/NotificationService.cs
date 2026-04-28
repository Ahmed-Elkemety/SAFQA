using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SAFQA.BLL.Managers.BackgroundServices;
using SAFQA.BLL.Managers.SellerAppManager.Notification;
using SAFQA.DAL.Database;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Managers.UserAppManager.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly SAFQA_Context _context;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(SAFQA_Context context,
                                   IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task SendAuctionFinishedNotification(
            int auctionId,
            decimal finalPrice,
            string winnerId,
            List<string> userIds)
        {
            // 📡 1. Real-time (SignalR)
            await _hub.Clients
                .Group($"auction-{auctionId}")
                .SendAsync("AuctionFinished", new
                {
                    auctionId,
                    finalPrice,
                    winnerId
                });

            // 💾 2. Notifications DB
            var notifications = new List<Notification>();

            foreach (var userId in userIds)
            {
                var isWinner = userId == winnerId;

                notifications.Add(new Notification
                {
                    UserId = userId,
                    Title = isWinner ? "🎉 You Won!" : "Auction Finished",
                    Message = isWinner
                        ? $"Congratulations! You won the auction. Final price: {finalPrice}"
                        : $"Auction has ended. Final price: {finalPrice}",
                    notificationType = NotificationTypes.AuctionsActivity,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });
            }

            await _context.Notifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
        }

        public async Task SendAuctionStatusUpdated(int auctionId, string status)
        {
            // ✅ 1. Get users from AuctionParticipations
            var userIds = await _context.auctionParticipations
                .Where(p => p.AuctionId == auctionId)
                .Select(p => p.UserId)
                .Distinct()
                .ToListAsync();

            // ✅ 2. Real-time SignalR (only active connected users in group)
            await _hub.Clients
                .Group($"auction-{auctionId}")
                .SendAsync("AuctionStatusUpdated", new
                {
                    auctionId,
                    status
                });

            // ✅ 3. Save notifications for offline users
            if (userIds.Any())
            {
                var notifications = userIds.Select(userId => new Notification
                {
                    Title = "Auction Update",
                    Message = $"Auction is now {status}",
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    notificationType = NotificationTypes.AuctionsActivity
                });

                await _context.Notifications.AddRangeAsync(notifications);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SendAuctionNotification(int auctionId, decimal price, string type)
        {
            var users = await _context.Bids
                .Where(b => b.AuctionId == auctionId)
                .Select(b => b.UserId)
                .Distinct()
                .ToListAsync();

            if (!users.Any())
                return;

            var title = type == "auto" ? "Auto Bid" : "New Bid";

            var notifications = users.Select(u => new Notification
            {
                UserId = u,
                Title = title,
                Message = $"Current price: {price}",
                notificationType = NotificationTypes.AuctionsActivity,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            });

            await _context.Notifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();

            await _hub.Clients
                .Group($"auction-{auctionId}")
                .SendAsync("ReceiveBid", new
                {
                    auctionId,
                    price,
                    type
                });
        }



        public async Task SendDisputeCreatedNotification(
            int disputeId,
            string sellerUserId,
            string title,
            string description,
            int ReferenceId)
        {
            
            await _hub.Clients
                .Group($"user-{sellerUserId}")
                .SendAsync("ReceiveDisputeNotification", new
                {
                    disputeId,
                    title,
                    description,
                    ReferenceId
                });

           
            var notification = new Notification
            {
                UserId = sellerUserId,
                Title = "New Dispute Created",
                Message = $"{title} - {description}",
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                notificationType = NotificationTypes.Dispute,
                ReferenceId = ReferenceId,
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }
    }
}
