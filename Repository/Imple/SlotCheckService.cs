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
                .Where(p => p.TourOperatorId == tourOperatorId && p.IsActive) 
                .OrderByDescending(p => p.ActivationDate)
                .Select(u => new CheckSlotTourOperatorResponse
                {
                    PurchaseId = u.PurchaseId,
                    TourOperatorId = u.TourOperatorId,
                    PackageId = u.PackageId,
                    EndDate = u.EndDate 
                })
                .FirstOrDefaultAsync();

            return activePackage;
        }
    }
}
