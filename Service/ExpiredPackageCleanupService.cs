using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;

namespace TourManagement_BE.Service
{
    public class ExpiredPackageCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Kiểm tra mỗi 24h

        public ExpiredPackageCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MyDBContext>();
                    await CheckAndUpdateExpiredPackages(dbContext);
                }
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckAndUpdateExpiredPackages(MyDBContext context)
        {
            var now = DateTime.UtcNow;
            var expiredPackages = await context.PurchasedServicePackages
                .Where(p => p.IsActive && p.EndDate < now)
                .ToListAsync();

            foreach (var p in expiredPackages)
            {
                p.IsActive = false;
            }

            if (expiredPackages.Count > 0)
            {
                await context.SaveChangesAsync();
            }
        }
    }
}
