using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Response.PaymentResponse;
using TourManagement_BE.Data.Models;

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
                    .ThenInclude(sp => sp.ServicePackageFeatures)
                .Where(p => p.TourOperatorId == tourOperatorId && p.IsActive && p.EndDate > DateTime.UtcNow.AddHours(7))
                .OrderByDescending(p => p.PackageId)
                .FirstOrDefaultAsync();

            if (activePackage == null) return null;

            var feature = activePackage.Package.ServicePackageFeatures.FirstOrDefault();
            //var numberOfTourAttribute = feature?.NumberOfTourAttribute ?? 0;
            //var numberoftours = feature?.NumberOfTours ?? 0;

            return new CheckSlotTourOperatorResponse
            {
                PurchaseId = activePackage.PurchaseId,
                TourOperatorId = activePackage.TourOperatorId,
                PackageId = activePackage.PackageId,
                EndDate = activePackage.EndDate,
                NumOfToursUsed = activePackage.NumOfToursUsed,
                //NumberOfTourAttribute = numberOfTourAttribute,
                //NumberOfTours = numberoftours,
                //PostVideo = feature?.PostVideo ?? false
            };
        }



    }
}
