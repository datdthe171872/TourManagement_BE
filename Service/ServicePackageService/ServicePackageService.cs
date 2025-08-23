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

        public async Task<ServiceResult> CreateAsync(CreateServicePackageRequest request)
        {
            var result = new ServiceResult();

            try
            {
                bool nameExists = await _context.ServicePackages.AnyAsync(sp => sp.Name.ToLower() == request.Name.ToLower());

                if (nameExists)
                {
                    result.Success = false;
                    result.Message = "Tên gói dịch vụ đã tồn tại";
                    return result;
                }

                var service = request.ToDto();
                await _context.ServicePackages.AddAsync(service);
                await _context.SaveChangesAsync();

                result.Data = service;
                result.Message = "Service package created successfully";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to create service package: {ex.Message}";
            }

            return result;
        }

        public async Task<ServiceResult> AddFeatureAsync(AddServicePackageFeatureRequest request)
        {
            var result = new ServiceResult();

            try
            {
                var packageExists = await _context.ServicePackages
                    .AnyAsync(p => p.PackageId == request.PackageId);

                if (!packageExists)
                {
                    result.Success = false;
                    result.Message = $"ServicePackage with ID {request.PackageId} not found";
                    return result;
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

                result.Data = newFeature;
                result.Message = "Feature added successfully";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to add feature: {ex.Message}";
            }

            return result;
        }

        public async Task<ServiceResult> UpdateAsync(UpdateServicePackageRequest request)
        {
            var result = new ServiceResult();

            try
            {
                var service = await _context.ServicePackages
                    .FirstOrDefaultAsync(u => u.PackageId == request.PackageId);

                if (service == null)
                {
                    result.Success = false;
                    result.Message = "Service Package not found";
                    return result;
                }

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

                result.Data = service;
                result.Message = "Service package updated successfully";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to update service package: {ex.Message}";
            }

            return result;
        }

        public async Task<ServiceResult> UpdateFeatureAsync(UpdateServicePackageFeatureRequest request)
        {
            var result = new ServiceResult();

            try
            {
                var feature = await _context.ServicePackageFeatures
                    .FirstOrDefaultAsync(f => f.FeatureId == request.FeatureId);

                if (feature == null)
                {
                    result.Success = false;
                    result.Message = $"Feature with ID {request.FeatureId} not found";
                    return result;
                }

                feature.FeatureName = request.FeatureName;
                feature.FeatureValue = request.FeatureValue;
                feature.IsActive = request.IsActive;

                await _context.SaveChangesAsync();

                result.Data = feature;
                result.Message = "Feature updated successfully";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to update feature: {ex.Message}";
            }

            return result;
        }

        public async Task<ServiceResult> ToggleStatusAsync(int packageId)
        {
            var result = new ServiceResult();

            try
            {
                var service = await _context.ServicePackages.FirstOrDefaultAsync(f => f.PackageId == packageId);
                if (service == null)
                {
                    result.Success = false;
                    result.Message = "Service package not found";
                    return result;
                }

                service.IsActive = !service.IsActive;
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = service;
                result.Message = $"Service package {(service.IsActive ? "activated" : "deactivated")} successfully";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to toggle status: {ex.Message}";
            }

            return result;
        }

        public async Task<ServiceResult> ToggleFeatureStatusAsync(int featureId)
        {
            var result = new ServiceResult();

            try
            {
                var feature = await _context.ServicePackageFeatures.FirstOrDefaultAsync(f => f.FeatureId == featureId);
                if (feature == null)
                {
                    result.Success = false;
                    result.Message = "Service package feature not found";
                    return result;
                }

                feature.IsActive = !feature.IsActive;
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = feature;
                result.Message = $"Feature {(feature.IsActive ? "activated" : "deactivated")} successfully";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to toggle feature status: {ex.Message}";
            }

            return result;
        }

        public async Task<ListServicePackageResponse> GetDetailForCustomerAsync(int packageId)
        {
            return await _context.ServicePackages.Include(p => p.ServicePackageFeatures)
                .Where(u => u.PackageId == packageId && u.IsActive)
                .Select(u => MapToResponse(u))
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Service package not found or inactive");
        }

        public async Task<ListServicePackageResponse> GetDetailForAdminAsync(int packageId)
        {
            return await _context.ServicePackages.Include(p => p.ServicePackageFeatures)
                .Where(u => u.PackageId == packageId)
                .Select(u => MapToResponse(u))
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Service package not found");
        }

        public async Task<CheckSlotTourOperatorResponse> CheckSlotForTourOperatorAsync(int userId)
        {
            try
            {
                var response = await _context.PurchasedServicePackages
                    .Include(p => p.TourOperator)
                        .ThenInclude(t => t.User)
                    .Include(p => p.Package)
                    .Where(p => p.TourOperator.User.UserId == userId &&
                           (p.EndDate == null || p.EndDate > DateTime.UtcNow) &&
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
                    .FirstOrDefaultAsync();

                if (response == null)
                {
                    throw new KeyNotFoundException("No active service package found for this user.");
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CheckSlotForTourOperatorAsync] Lỗi với userId {userId}: {ex.Message}");
                throw;
            }
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
                ServicePackageFeaturesResponses = package.ServicePackageFeatures?
                    .Where(f => f.IsActive)
                    .Select(f => new ServicePackageFeaturesResponse
                    {
                        FeatureId = f.FeatureId,
                        PackageId = f.PackageId,
                        FeatureName = f.FeatureName,
                        FeatureValue = f.FeatureValue, 
                        IsActive = f.IsActive
                    })
                    .ToList() ?? new List<ServicePackageFeaturesResponse>()
            };
        }

    }
}
