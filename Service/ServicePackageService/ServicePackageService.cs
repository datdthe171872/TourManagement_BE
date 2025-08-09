using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.ServicePackageRequest;
using TourManagement_BE.Data.DTO.Response.PaymentResponse;
using TourManagement_BE.Data.DTO.Response.ServicePackage;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Mapping.ServiceMapping;

namespace TourManagement_BE.Service.ServicePackageService
{
    public class ServicePackageService : IServicePackageService
    {
        private readonly MyDBContext _context;
        private readonly IMapper _mapper;

        public ServicePackageService(MyDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ListServicePackageResponse>> ListAllForAdminAsync()
        {
            return await _context.ServicePackages
                .Select(u => MapToResponse(u))
                .ToListAsync();
        }

        public async Task<PagedResult<ListServicePackageResponse>> ListAllPaginatedForAdminAsync(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var totalRecords = await _context.ServicePackages.CountAsync();
            var data = await _context.ServicePackages
                .Select(u => MapToResponse(u))
                .ToListAsync();

            return new PagedResult<ListServicePackageResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = data
            };
        }

        public async Task<List<ListServicePackageResponse>> ListAllForCustomerAsync()
        {
            return await _context.ServicePackages
                .Where(u => u.IsActive)
                .Select(u => MapToResponse(u))
                .ToListAsync();
        }

        public async Task<PagedResult<ListServicePackageResponse>> ListAllPaginatedForCustomerAsync(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _context.ServicePackages.Where(u => u.IsActive);
            var totalRecords = await query.CountAsync();
            var data = await query
                .Select(u => MapToResponse(u))
                .ToListAsync();

            return new PagedResult<ListServicePackageResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = data
            };
        }

        public async Task CreateAsync(CreateServicePackageRequest request)
        {
            var service = request.ToDto();
            await _context.ServicePackages.AddAsync(service);
            await _context.SaveChangesAsync();
        }

        public async Task AddFeatureAsync(AddServicePackageFeatureRequest request)
        {
            var packageExists = await _context.ServicePackages
                .AnyAsync(p => p.PackageId == request.PackageId);

            if (!packageExists)
            {
                throw new KeyNotFoundException($"ServicePackage with ID {request.PackageId} not found");
            }

            var newFeature = new ServicePackageFeature
            {
                PackageId = request.PackageId,
                FeatureName = request.FeatureName,
                FeatureValue = request.FeatureValue,
                IsActive = request.IsActive
            };

            await _context.ServicePackageFeatures.AddAsync(newFeature);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UpdateServicePackageRequest request)
        {
            var service = await _context.ServicePackages
                .FirstOrDefaultAsync(u => u.PackageId == request.PackageId)
                ?? throw new KeyNotFoundException("Service Package not found.");

            service.Name = request.Name;
            service.Description = request.Description;
            service.Price = request.Price;
            service.DiscountPercentage = request.DiscountPercentage;
            service.MaxTour = request.MaxTour;
            service.MaxImage = request.MaxImage;
            service.MaxVideo = request.MaxVideo;
            service.TourGuideFunction = request.TourGuideFunction;
            service.IsActive = request.IsActive;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateFeatureAsync(UpdateServicePackageFeatureRequest request)
        {
            var feature = await _context.ServicePackageFeatures
                .FirstOrDefaultAsync(f => f.FeatureId == request.FeatureId)
                ?? throw new KeyNotFoundException($"Feature with ID {request.FeatureId} not found");

            feature.FeatureName = request.FeatureName;
            feature.FeatureValue = request.FeatureValue;
            feature.IsActive = request.IsActive;

            await _context.SaveChangesAsync();
        }

        public async Task ToggleStatusAsync(int packageId)
        {
            var service = await _context.ServicePackages.FindAsync(packageId)
                ?? throw new KeyNotFoundException("Service package not found.");

            service.IsActive = !service.IsActive;
            await _context.SaveChangesAsync();
        }

        public async Task ToggleFeatureStatusAsync(int featureId)
        {
            var feature = await _context.ServicePackageFeatures.FindAsync(featureId)
                ?? throw new KeyNotFoundException("Service package feature not found.");

            feature.IsActive = !feature.IsActive;
            await _context.SaveChangesAsync();
        }

        public async Task<ListServicePackageResponse> GetDetailForCustomerAsync(int packageId)
        {
            return await _context.ServicePackages
                .Where(u => u.PackageId == packageId && u.IsActive)
                .Select(u => MapToResponse(u))
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Service package not found or inactive");
        }

        public async Task<ListServicePackageResponse> GetDetailForAdminAsync(int packageId)
        {
            return await _context.ServicePackages
                .Where(u => u.PackageId == packageId)
                .Select(u => MapToResponse(u))
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Service package not found");
        }

        public async Task<CheckSlotTourOperatorResponse> CheckSlotForTourOperatorAsync(int userId)
        {
            return await _context.PurchasedServicePackages
                .Include(p => p.TourOperator)
                    .ThenInclude(t => t.User)
                .Include(p => p.Package)
                .Where(p => p.TourOperator.User.UserId == userId &&
                        p.EndDate > DateTime.UtcNow &&
                        p.IsActive)
                .OrderByDescending(p => p.ActivationDate)
                .Select(p => new CheckSlotTourOperatorResponse
                {
                    PurchaseId = p.PurchaseId,
                    TourOperatorId = p.TourOperatorId,
                    TourOperatorName = p.TourOperator.User.UserName,
                    PackageId = p.PackageId,
                    PackageName = p.Package.Name,
                    TransactionId = p.TransactionId,
                    ActivationDate = p.ActivationDate,
                    EndDate = p.EndDate,
                    NumOfToursUsed = p.NumOfToursUsed,
                    MaxTour = p.Package.MaxTour,
                    MaxImage = p.Package.MaxImage,
                    MaxVideo = p.Package.MaxVideo,
                    TourGuideFunction = p.Package.TourGuideFunction,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt
                })
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("No active service package found for this user.");
        }

        private static ListServicePackageResponse MapToResponse(ServicePackage package)
        {
            return new ListServicePackageResponse
            {
                PackageId = package.PackageId,
                Name = package.Name,
                Description = package.Description,
                Price = package.Price,
                DiscountPercentage = package.DiscountPercentage,
                TotalPrice = (decimal)(package.Price - package.Price * (package.DiscountPercentage / 100)),
                MaxTour = package.MaxTour,
                MaxImage = package.MaxImage,
                MaxVideo = package.MaxVideo,
                TourGuideFunction = package.TourGuideFunction,
                IsActive = package.IsActive,
            };
        }
    }
}
