using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
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
                .Include(p => p.Package)
                .Where(p => p.TourOperatorId == tourOperatorId && p.IsActive && p.EndDate > DateTime.UtcNow.AddHours(7))
                .OrderByDescending(p => p.PackageId)
                .FirstOrDefaultAsync();

            if (activePackage != null)
            {
                var package = activePackage.Package;

                return new CheckSlotTourOperatorResponse
                {
                    PurchaseId = activePackage.PurchaseId,
                    TourOperatorId = activePackage.TourOperatorId,
                    PackageId = activePackage.PackageId,
                    EndDate = activePackage.EndDate,
                    NumOfToursUsed = activePackage.NumOfToursUsed,
                    MaxTour = package.MaxTour,
                    MaxImage = package.MaxImage,
                    MaxVideo = package.MaxVideo,
                    TourGuideFunction = package.TourGuideFunction,
                    IsActive = activePackage.IsActive,
                    CreatedAt = activePackage.CreatedAt
                };
            }

            // Nếu không có gói mua, fallback sang gói miễn phí
            var freePackage = await _context.ServicePackages
                .Where(p => p.PackageId == 1 && p.IsActive)
                .FirstOrDefaultAsync();

            if (freePackage == null)
                return null;

            // ✅ Tính số tour đã tạo trong tháng hiện tại
            var now = DateTime.UtcNow.AddHours(7);
            var monthTours = await _context.Tours
                .Where(t => t.TourOperatorId == tourOperatorId
                    && t.CreatedAt.HasValue
                    && t.CreatedAt.Value.Month == now.Month
                    && t.CreatedAt.Value.Year == now.Year
                    && t.IsActive)
                .CountAsync();

            return new CheckSlotTourOperatorResponse
            {
                PurchaseId = 0,
                TourOperatorId = tourOperatorId,
                PackageId = freePackage.PackageId,
                EndDate = DateTime.MaxValue,
                NumOfToursUsed = monthTours, // ✅ gán số tour tháng này
                MaxTour = freePackage.MaxTour,
                MaxImage = freePackage.MaxImage,
                MaxVideo = freePackage.MaxVideo,
                TourGuideFunction = freePackage.TourGuideFunction,
                IsActive = true,
                CreatedAt = now
            };
        }

    }
}
