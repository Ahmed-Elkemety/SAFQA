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
                try
                {
                    await ProcessEscrowPayments(stoppingToken);
                }
                catch (Exception ex)
                {
                    // تسجيل الخطأ العام للخدمة
                    Console.WriteLine($"Critical BackgroundService Error: {ex.Message}");
                }

                // الانتظار لمدة 10 دقائق قبل الدورة القادمة
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        private async Task ProcessEscrowPayments(CancellationToken cancellationToken)
        {
            using var rootScope = _scopeFactory.CreateScope();
            var rootContext = rootScope.ServiceProvider.GetRequiredService<SAFQA_Context>();

            var threeDaysAgo = DateTime.UtcNow.AddDays(-3);

            var auctionIds = await rootContext.Auctions
                .Where(a => a.Status == AuctionStatus.Finished &&
                            !a.IsDeleted &&
                            !a.IsEscrowReleased &&
                            a.EndDate <= threeDaysAgo)
                .Select(a => a.Id)
                .Take(50) 
                .ToListAsync(cancellationToken);

            if (!auctionIds.Any())
                return;

            foreach (var auctionId in auctionIds)
            {
                using var innerScope = _scopeFactory.CreateScope();
                var context = innerScope.ServiceProvider.GetRequiredService<SAFQA_Context>();
                var hub = innerScope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

                using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    var auction = await context.Auctions
                        .Include(a => a.Seller)
                        .Include(a => a.disputes)
                        .FirstOrDefaultAsync(a => a.Id == auctionId, cancellationToken);

                    if (auction == null || auction.disputes.Any() || string.IsNullOrEmpty(auction.WinnerUserId))
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        continue;
                    }

                    var buyerWallet = await context.Wallets.FirstOrDefaultAsync(w => w.UserId == auction.WinnerUserId, cancellationToken);
                    var sellerWallet = await context.Wallets.FirstOrDefaultAsync(w => w.UserId == auction.Seller.UserId, cancellationToken);

                    if (buyerWallet == null || sellerWallet == null)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        continue;
                    }

                    decimal amount = auction.FinalPrice;

                    buyerWallet.FrozenBalance -= amount;

                    var sellerBefore = sellerWallet.Balance;
                    sellerWallet.Balance += amount;
                    sellerWallet.UpdatedAt = DateTime.UtcNow;

                    auction.IsEscrowReleased = true;

                    context.Transactions.Add(new Transactions
                    {
                        Type = TransactionType.Sale,
                        Status = TransactionStatus.Completed,
                        WalletId = sellerWallet.Id,
                        Amount = amount,
                        BalanceBefore = sellerBefore,
                        BalanceAfter = sellerWallet.Balance,
                        Description = $"Escrow release for Auction #{auction.Id}",
                        CreatedAt = DateTime.UtcNow
                    });

                    await context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    await NotifyUsers(hub, auction, sellerWallet.Balance, amount);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    Console.WriteLine($"Error processing Auction {auctionId}: {ex.Message}");
                }
            }
        }

        private async Task NotifyUsers(IHubContext<NotificationHub> hub, Auction auction, decimal newBalance, decimal amount)
        {
            try
            {
                await hub.Clients.Group($"user-{auction.Seller.UserId}")
                    .SendAsync("WalletUpdated", new
                    {
                        message = $"+{amount} received from Auction #{auction.Id}",
                        balance = newBalance
                    });

                await hub.Clients.Group($"user-{auction.WinnerUserId}")
                    .SendAsync("PaymentReleased", new
                    {
                        message = $"Payment released for Auction #{auction.Id}"
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Notification Error: {ex.Message}");
            }
        }
    }
}
