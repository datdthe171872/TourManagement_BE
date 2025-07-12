using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Response.PaymentResponse;

namespace TourManagement_BE.Repository.Interface
{
    public class SlotCheckService : ISlotCheckService
    {
        private readonly MyDBContext _context;

        public SlotCheckService(MyDBContext context)
        {
            _context = context;
        }

        public async Task<CheckSlotTourOperatorResponse?> CheckRemainingSlotsAsync(int tourOperatorId)
        {
            var activePackage = await _context.PurchasedServicePackages
                .Where(p => p.TourOperatorId == tourOperatorId && p.EndDate > DateTime.UtcNow && p.IsActive)
                .OrderByDescending(p => p.ActivationDate)
                .Select(u => new CheckSlotTourOperatorResponse
                {
                    PurchaseId = u.PackageId,
                    TourOperatorId = u.TourOperatorId,
                    PackageId = u.PackageId,
                    MaxTour = u.Package.MaxTours,
                    NumOfToursUsed = u.NumOfToursUsed,
                    RemainingTours = u.Package.MaxTours - u.NumOfToursUsed,
                })
                .FirstOrDefaultAsync();

            return activePackage;
        }
    }
}
