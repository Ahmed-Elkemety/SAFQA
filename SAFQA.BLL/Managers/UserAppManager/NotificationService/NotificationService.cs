using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Managers.BackgroundServices;
using SAFQA.BLL.Managers.SellerAppManager.Notification;
using SAFQA.DAL.Database;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;

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

        public async Task SendAuctionStatusUpdated(int auctionId, string status, List<string> userIds)
        {
            // ✅ 1. Real-time via SignalR
            await _hub.Clients.Group($"auction-{auctionId}")
                .SendAsync("AuctionStatusUpdated", new
                {
                    auctionId,
                    status
                });

            // ✅ 2. Save in DB (for offline users)
            foreach (var userId in userIds)
            {
                _context.Notifications.Add(new Notification
                {
                    Title = "Auction Update",
                    Message = $"Auction is now {status}",
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task SendAuctionNotification(int auctionId, decimal price, string type)
        {
            var users = await _context.Bids
                .Where(b => b.AuctionId == auctionId)
                .Select(b => b.UserId)
                .Distinct()
                .ToListAsync();

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

            // 📡 SignalR
            await _hub.Clients
                .Group($"auction-{auctionId}")
                .SendAsync("ReceiveBid", new
                {
                    auctionId,
                    price,
                    type
                });
        }
    }
}
