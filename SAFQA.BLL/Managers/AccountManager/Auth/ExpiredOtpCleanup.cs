using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAFQA.DAL.Database;

namespace SAFQA.BLL.Managers.AccountManager.Auth
{
    public class ExpiredOtpCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5); 

        public ExpiredOtpCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<SAFQA_Context>();

                var expiredOtps = context.PasswordResetOtps
                    .Where(x => x.Expiration <= DateTime.UtcNow);

                if (expiredOtps.Any())
                {
                    context.PasswordResetOtps.RemoveRange(expiredOtps);
                    await context.SaveChangesAsync();
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
