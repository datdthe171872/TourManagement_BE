using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.ServicePackageRequest;
using TourManagement_BE.Data.DTO.Response.PaymentResponse;
using TourManagement_BE.Data.DTO.Response.ServicePackage;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Mapping.ServiceMapping;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicePackageController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;

        public ServicePackageController(MyDBContext context, IMapper mapper)
        {
            this.context = context;
            _mapper = mapper;
        }

        [HttpGet("ListAllServicePackageForAdmin")]
        public IActionResult ListAllServicePackageForAdmin()
        {
            var services = context.ServicePackages.
                Select(u => new ListServicePackageResponse
                {
                    PackageId = u.PackageId,
                    Name = u.Name,
                    Description = u.Description,
                    Price = u.Price,
                    DiscountPercentage = u.DiscountPercentage,
                    TotalPrice = (decimal)(u.Price - (u.Price * (u.DiscountPercentage / 100))),
                    IsActive = u.IsActive,
                    ServicePackageFeaturesResponses = u.ServicePackageFeatures.Where(f => f.IsActive)
                        .Select(f => new ServicePackageFeaturesResponse
                        {
                            FeatureId = f.FeatureId,
                            PackageId = f.PackageId,
                            NumberOfTours = f.NumberOfTours,
                            NumberOfTourAttribute = f.NumberOfTourAttribute,
                            PostVideo = f.PostVideo,
                            IsActive = f.IsActive
                        }).ToList()
                }).ToList();

            if (!services.Any())
            {
                return NotFound("Not Found.");
            }

            return Ok(services);
        }

        [HttpGet("ListAllServicePackagePaggingForAdmin")]
        public IActionResult ListAllServicePackagePaggingForAdmin(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalRecords = context.ServicePackages.Count();

            var services = context.ServicePackages.
                Select(u => new ListServicePackageResponse
                {
                    PackageId = u.PackageId,
                    Name = u.Name,
                    Description = u.Description,
                    Price = u.Price,
                    DiscountPercentage = u.DiscountPercentage,
                    TotalPrice = (decimal)(u.Price - (u.Price * (u.DiscountPercentage / 100))),
                    IsActive = u.IsActive,
                    ServicePackageFeaturesResponses = u.ServicePackageFeatures.Where(f => f.IsActive)
                        .Select(f => new ServicePackageFeaturesResponse
                        {
                            FeatureId = f.FeatureId,
                            PackageId = f.PackageId,
                            NumberOfTours = f.NumberOfTours,
                            NumberOfTourAttribute = f.NumberOfTourAttribute,
                            PostVideo = f.PostVideo,
                            IsActive = f.IsActive
                        }).ToList()
                }).ToList();

            if (!services.Any())
            {
                return NotFound("Not Found.");
            }

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = services
            });
        }


        [HttpGet("ListAllServicePackageForCustomer")]
        public IActionResult ListAllServicePackageForCustomer()
        {
            var services = context.ServicePackages.
                Select(u => new ListServicePackageResponse
                {
                    PackageId = u.PackageId,
                    Name = u.Name,
                    Description = u.Description,
                    Price = u.Price,
                    DiscountPercentage = u.DiscountPercentage,
                    TotalPrice = (decimal)(u.Price - (u.Price * (u.DiscountPercentage / 100))),
                    IsActive = true,
                    ServicePackageFeaturesResponses = u.ServicePackageFeatures.Where(f => f.IsActive)
                        .Select(f => new ServicePackageFeaturesResponse
                        {
                            FeatureId = f.FeatureId,
                            PackageId = f.PackageId,
                            NumberOfTours = f.NumberOfTours,
                            NumberOfTourAttribute = f.NumberOfTourAttribute,
                            PostVideo = f.PostVideo,
                            IsActive = true
                        }).ToList()
                }).ToList();


            if (!services.Any())
            {
                return NotFound("Not Found.");
            }

            return Ok(services);
        }

        [HttpGet("ListAllServicePackagePaggingForCustomer")]
        public IActionResult ListAllServicePackagePagging(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalRecords = context.ServicePackages.Count();

            var services = context.ServicePackages.
                Select(u => new ListServicePackageResponse
                {
                    PackageId = u.PackageId,
                    Name = u.Name,
                    Description = u.Description,
                    Price = u.Price,
                    DiscountPercentage = u.DiscountPercentage,
                    TotalPrice = (decimal)(u.Price - (u.Price * (u.DiscountPercentage / 100))),
                    IsActive = true,
                    ServicePackageFeaturesResponses = u.ServicePackageFeatures.Where(f => f.IsActive)
                        .Select(f => new ServicePackageFeaturesResponse
                        {
                            FeatureId = f.FeatureId,
                            PackageId = f.PackageId,
                            NumberOfTours = f.NumberOfTours,
                            NumberOfTourAttribute = f.NumberOfTourAttribute,
                            PostVideo = f.PostVideo,
                            IsActive = true
                        }).ToList()
                }).ToList();

            if (!services.Any())
            {
                return NotFound("Not Found.");
            }

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = services
            });
        }


        [HttpPost("CreateServicePackage")]
        public IActionResult CreateServicePackage([FromBody] CreateServicePackageRequest request)
        {
            try
            {
                var service = request.ToDto();
                context.ServicePackages.Add(service);
                context.SaveChanges();
                return Ok(new { message = "Create Service Package successfully." });
            }
            catch (Exception ex)
            {
                return Ok(new { message = "Create Service Package Fail." });
            }
        }

        [HttpPost("AddServicePackageFeature")]
        public async Task<IActionResult> AddServicePackageFeature([FromBody] AddServicePackageFeatureRequest request)
        {
            try
            {
                var packageExists = await context.ServicePackages
                    .AnyAsync(p => p.PackageId == request.PackageId);

                if (!packageExists)
                {
                    return NotFound(new { Message = $"ServicePackage with ID {request.PackageId} not found" });
                }

                var newFeature = new ServicePackageFeature
                {
                    PackageId = request.PackageId,
                    NumberOfTours = request.NumberOfTours,
                    NumberOfTourAttribute = request.NumberOfTourAttribute,
                    PostVideo = request.PostVideo,
                    IsActive = request.IsActive
                };

                context.ServicePackageFeatures.Add(newFeature);
                await context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "ServicePackageFeature added successfully",
                    FeatureId = newFeature.FeatureId,
                    PackageId = newFeature.PackageId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Failed to add ServicePackageFeature",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("UpdateServicePackage")]
        public async Task<IActionResult> UpdateServicePackage([FromBody] UpdateServicePackageRequest request)
        {
            var service = context.ServicePackages.FirstOrDefault(u => u.PackageId == request.PackageId);
            if (service == null)
            {
                return NotFound("Service Package not found.");
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
                service.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Description))
                service.Description = request.Description;

            if (request.Price.HasValue)
                service.Price = request.Price.Value;

            if (request.DiscountPercentage.HasValue)
                service.DiscountPercentage = request.DiscountPercentage.Value;

            if (request.IsActive.HasValue)
                service.IsActive = request.IsActive.Value;

            await context.SaveChangesAsync();

            return Ok(new { message = "Service package updated successfully." });
        }

        [HttpPut("UpdateServicePackageFeature")]
        public async Task<IActionResult> UpdateServicePackageFeature([FromBody] UpdateServicePackageFeatureRequest request)
        {
            try
            { 
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var feature = await context.ServicePackageFeatures
                    .FirstOrDefaultAsync(f => f.FeatureId == request.FeatureId);

                if (feature == null)
                {
                    return NotFound(new { Message = $"Feature with ID {request.FeatureId} not found" });
                }

                feature.NumberOfTours = request.NumberOfTours;
                feature.NumberOfTourAttribute = request.NumberOfTourAttribute;
                feature.PostVideo = request.PostVideo;
                feature.IsActive = request.IsActive;

                context.ServicePackageFeatures.Update(feature);
                await context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Feature updated successfully",
                    FeatureId = feature.FeatureId,
                    PackageId = feature.PackageId
                });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi ở đây
                return StatusCode(500, new
                {
                    Message = "Failed to update feature",
                    Error = ex.Message
                });
            }
        }

        [HttpPatch("ToggleServicePackageStatus/{packageId}")]
        public async Task<IActionResult> ToggleServicePackageStatus(int packageId)
        {
            var service = await context.ServicePackages.FindAsync(packageId);
            if (service == null)
            {
                return NotFound("Service package not found.");
            }

            // Toggle trạng thái IsActive
            service.IsActive = !service.IsActive;

            await context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Service package has been {(service.IsActive ? "activated" : "deactivated")}",
                packageId = packageId,
                newStatus = service.IsActive
            });
        }

        [HttpPatch("ToggleServicePackageFeatureStatus/{featureid}")]
        public async Task<IActionResult> ToggleServicePackageFeatureStatus(int featureid)
        {
            var service = await context.ServicePackageFeatures.FindAsync(featureid);
            if (service == null)
            {
                return NotFound("Service package feature not found.");
            }

            service.IsActive = !service.IsActive;

            await context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Service package has been {(service.IsActive ? "activated" : "deactivated")}",
                featureid = featureid,
                newStatus = service.IsActive
            });
        }


        [HttpGet("ViewDetailPackageServiceForCustomer/{packageid}")]
        public IActionResult ViewDetailPackageServicForCustomere(int packageid)
        {
            var services = context.ServicePackages
                .Where(u => u.PackageId == packageid)
                .Select(u => new ListServicePackageResponse
                {
                    PackageId = u.PackageId,
                    Name = u.Name,
                    Description = u.Description,
                    Price = u.Price,
                    DiscountPercentage = u.DiscountPercentage,
                    TotalPrice = (decimal)(u.Price - (u.Price * (u.DiscountPercentage / 100))),
                    IsActive = true,
                    ServicePackageFeaturesResponses = u.ServicePackageFeatures.Where(f => f.IsActive)
                        .Select(f => new ServicePackageFeaturesResponse
                        {
                            FeatureId = f.FeatureId,
                            PackageId = f.PackageId,
                            NumberOfTours = f.NumberOfTours,
                            NumberOfTourAttribute = f.NumberOfTourAttribute,
                            PostVideo = f.PostVideo,
                            IsActive = true
                        }).ToList()
                })
                .FirstOrDefault();

            if (services == null)
            {
                return NotFound("Not found.");
            }

            return Ok(services);
        }

        [HttpGet("ViewDetailPackageServiceForAdmin/{packageid}")]
        public IActionResult ViewDetailPackageServiceForAdmin(int packageid)
        {
            var services = context.ServicePackages
                .Where(u => u.PackageId == packageid)
                .Select(u => new ListServicePackageResponse
                {
                    PackageId = u.PackageId,
                    Name = u.Name,
                    Description = u.Description,
                    Price = u.Price,
                    DiscountPercentage = u.DiscountPercentage,
                    TotalPrice = (decimal)(u.Price - (u.Price * (u.DiscountPercentage / 100))),
                    IsActive = u.IsActive,
                    ServicePackageFeaturesResponses = u.ServicePackageFeatures.Where(f => f.IsActive)
                        .Select(f => new ServicePackageFeaturesResponse
                        {
                            FeatureId = f.FeatureId,
                            PackageId = f.PackageId,
                            NumberOfTours = f.NumberOfTours,
                            NumberOfTourAttribute = f.NumberOfTourAttribute,
                            PostVideo = f.PostVideo,
                            IsActive = f.IsActive
                        }).ToList()
                })
                .FirstOrDefault();

            if (services == null)
            {
                return NotFound("Not found.");
            }

            return Ok(services);
        }

        [HttpGet("CheckSlotTourOperatorPackageService/{userid}")]
        public IActionResult CheckSlotTourOperatorPackageService(int userid)
        {
            var activePackage = context.PurchasedServicePackages.Include(t => t.TourOperator).ThenInclude(u => u.User)
                .Where(p => p.TourOperator.User.UserId == userid && p.EndDate > DateTime.UtcNow && p.IsActive)
                .OrderByDescending(p => p.ActivationDate)
                .Select(u => new CheckSlotTourOperatorResponse
                {
                    PurchaseId = u.PackageId,
                    TourOperatorId = u.TourOperatorId,
                    TourOperatorName = u.TourOperator.User.UserName,
                    PackageId = u.PackageId,
                    PackageName = u.Package.Name,
                    TransactionId = u.TransactionId,
                    ActivationDate = u.ActivationDate,
                    EndDate = u.EndDate,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefault();

            if (activePackage == null)
            {
                return NotFound("Not found.");
            }

            return Ok(activePackage);
        }
    }
}
