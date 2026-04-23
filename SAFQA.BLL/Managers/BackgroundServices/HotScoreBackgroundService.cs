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
    public class HotScoreBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public HotScoreBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var manager = scope.ServiceProvider.GetRequiredService<IAuctionManagerU>();

                    try
                    {
                        await manager.CalculateHotScoresAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
