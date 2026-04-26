using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAFQA.DAL.Database;
using SAFQA.DAL.Enums;
using SAFQA.DAL.Models;

namespace SAFQA.BLL.Managers.BackgroundServices
{
    public class EscrowReleaseBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EscrowReleaseBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessEscrowPayments();

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        private async Task ProcessEscrowPayments()
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<SAFQA_Context>();
            var hub = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

            var threeDaysAgo = DateTime.UtcNow.AddDays(-3);

            var auctions = await context.Auctions
                .Include(a => a.Seller)
                .Include(a => a.disputes)
                .Where(a =>
                    a.Status == AuctionStatus.Finished &&
                    !a.IsDeleted &&
                    !a.IsEscrowReleased &&
                    a.EndDate <= threeDaysAgo)
                .ToListAsync();

            foreach (var auction in auctions)
            {
                if (auction.disputes.Any())
                    continue;

                if (string.IsNullOrEmpty(auction.WinnerUserId))
                    continue;

                var buyerWallet = await context.Wallets
                    .FirstOrDefaultAsync(w => w.UserId == auction.WinnerUserId);

                var sellerWallet = await context.Wallets
                    .FirstOrDefaultAsync(w => w.UserId == auction.Seller.UserId);

                if (buyerWallet == null || sellerWallet == null)
                    continue;

                decimal amount = auction.FinalPrice;

                buyerWallet.FrozenBalance -= amount;

                var sellerBefore = sellerWallet.Balance;

                sellerWallet.Balance += amount;
                sellerWallet.UpdatedAt = DateTime.UtcNow;

                // 🧾 Transaction
                context.Transactions.Add(new Transactions
                {
                    Type = TransactionType.Sale,
                    Status = TransactionStatus.Completed,
                    WalletId = sellerWallet.Id,
                    Amount = amount,
                    BalanceBefore = sellerBefore,
                    BalanceAfter = sellerWallet.Balance,
                    Description = $"Sale from Auction #{auction.Id}",
                    CreatedAt = DateTime.UtcNow
                });
                auction.IsEscrowReleased = true;

                // 🔔 Seller Notification
                await hub.Clients.Group($"user-{auction.Seller.UserId}")
                     .SendAsync("WalletUpdated", new
                     {
                         message = $"+{amount} received from Auction #{auction.Id}",
                         balance = sellerWallet.Balance
                     });

                // 🔔 Buyer Notification
                await hub.Clients.Group($"user-{auction.WinnerUserId}")
                    .SendAsync("PaymentReleased", new
                    {
                        message = $"Payment released for Auction #{auction.Id}"
                    });
            }

            await context.SaveChangesAsync();
        }
    }
}
