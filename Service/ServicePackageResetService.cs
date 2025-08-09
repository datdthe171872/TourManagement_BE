using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;

namespace TourManagement_BE.Service
{
    public class ServicePackageResetService
    {
        private readonly MyDBContext _context;

        public ServicePackageResetService(MyDBContext context)
        {
            _context = context;
        }

        public async Task ResetMonthlyTourCountAsync()
        {
            var now = DateTime.UtcNow.AddHours(7);

            var activePackages = await _context.PurchasedServicePackages
                .Where(x => x.IsActive && x.ActivationDate <= now && x.EndDate >= now)
                .ToListAsync();

            foreach (var package in activePackages)
            {
                package.NumOfToursUsed = 0;
            }

            await _context.SaveChangesAsync();
        }
    }
}
