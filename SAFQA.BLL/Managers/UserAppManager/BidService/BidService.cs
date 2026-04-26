using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SAFQA.BLL.Dtos.UserAppDto.BidDto;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.BLL.Managers.BackgroundServices;
using SAFQA.BLL.Managers.SellerAppManager.BidService;
using SAFQA.BLL.Managers.SellerAppManager.Notification;
using SAFQA.BLL.Managers.UserAppManager.NotificationService;
using SAFQA.DAL.Database;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;

namespace SAFQA.BLL.Managers.UserAppManager.BidService
{
    using System.Collections.Concurrent;
    using System.Data;
    using Microsoft.EntityFrameworkCore;

    public class BidService : IBidService
    {
        private readonly SAFQA_Context _context;
        private readonly INotificationService _notification;

        // 🔒 Lock per Auction
        private static readonly ConcurrentDictionary<int, SemaphoreSlim> _locks = new();

        public BidService(SAFQA_Context context, INotificationService notification)
        {
            _context = context;
            _notification = notification;
        }

        public async Task PlaceManualBid(string userId, int auctionId, decimal amount)
        {
            var auctionLock = _locks.GetOrAdd(auctionId, _ => new SemaphoreSlim(1, 1));

            await auctionLock.WaitAsync();

            decimal manualPrice = amount;
            decimal finalPrice = amount;
            bool hasProxy = false;

            try
            {
                await using var transaction = await _context.Database
                    .BeginTransactionAsync(IsolationLevel.ReadCommitted);

                var auction = await _context.Auctions
                    .FirstOrDefaultAsync(a => a.Id == auctionId);

                if (auction == null)
                    throw new Exception("Auction not found");

                if (auction.EndDate <= DateTime.UtcNow)
                    throw new Exception("Auction is closed");

                // 💰 WALLET CHECK (Manual)
                if (!await HasEnoughBalance(userId, amount))
                    throw new Exception("Insufficient balance");

                if (amount < auction.CurrentPrice + auction.BidIncrement)
                    throw new Exception("Outbid - price changed");

                // ✅ Manual Bid
                _context.Bids.Add(new Bid
                {
                    UserId = userId,
                    AuctionId = auctionId,
                    Amount = amount,
                    Type = BidType.Manual,
                    Date = DateTime.UtcNow
                });
                auction.TotalBids++;
                auction.CurrentPrice = amount;
                auction.UpdatedAt = DateTime.UtcNow;

                // 🤖 Proxy Logic
                (finalPrice, hasProxy) = await HandleProxyTop2(auctionId, userId, amount);

                auction.CurrentPrice = finalPrice;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            finally
            {
                auctionLock.Release();
            }

            await _notification.SendAuctionNotification(auctionId, manualPrice, "manual");

            if (hasProxy)
            {
                await _notification.SendAuctionNotification(auctionId, finalPrice, "auto");
            }
        }

        private async Task<(decimal price, bool hasProxy)> HandleProxyTop2(int auctionId, string manualUserId, decimal currentPrice)
        {
            var proxies = await _context.proxyBiddings
                .Where(p => p.AuctionId == auctionId && p.Status == ProxyStatus.Active)
                .OrderByDescending(p => p.Max)
                .Take(2)
                .ToListAsync();

            if (proxies.Count == 0)
                return (currentPrice, false);

            var first = proxies[0];

            // لو نفس اليوزر → مفيش Proxy
            if (first.UserId == manualUserId)
                return (currentPrice, false);

            // 💰 WALLET CHECK (Proxy Owner)
            if (!await HasEnoughBalance(first.UserId, first.Max))
            {
                first.Status = ProxyStatus.Expired;
                await _context.SaveChangesAsync();
                return (currentPrice, false);
            }

            if (currentPrice >= first.Max)
            {
                first.Status = ProxyStatus.Expired;
                return (currentPrice, false);
            }

            decimal newPrice;

            if (proxies.Count == 1)
            {
                newPrice = currentPrice + first.Step;
            }
            else
            {
                var second = proxies[1];
                newPrice = second.Max + first.Step;
            }


            if (newPrice > first.Max)
            {
                newPrice = first.Max;
                first.Status = ProxyStatus.Expired;
            }

            _context.Bids.Add(new Bid
            {
                UserId = first.UserId,
                AuctionId = auctionId,
                Amount = newPrice,
                Type = BidType.Auto,
                ProxyBiddingId = first.Id,
                Date = DateTime.UtcNow
            });
            first.auction.TotalBids++;

            return (newPrice, true);
        }

        private async Task<bool> HasEnoughBalance(string userId, decimal amount)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wallet == null)
                return false;

            return wallet.Balance >= amount;
        }
    }
}
