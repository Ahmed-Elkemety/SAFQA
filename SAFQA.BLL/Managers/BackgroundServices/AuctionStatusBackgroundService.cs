using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAFQA.BLL.Managers.UserAppManager.AuctionManager;

namespace SAFQA.BLL.Managers.BackgroundServices
{
    public class AuctionStatusBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AuctionStatusBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var manager = scope.ServiceProvider
                    .GetRequiredService<IAuctionManagerU>();

                await manager.UpdateAuctionStatusesAsync();

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
