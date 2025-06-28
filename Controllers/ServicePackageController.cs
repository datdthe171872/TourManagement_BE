using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.ProfileRequest;
using TourManagement_BE.Data.DTO.Request.ServicePackageRequest;
using TourManagement_BE.Data.DTO.Response.ServicePackage;
using TourManagement_BE.Data.DTO.Response.TourResponse;
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

        [HttpGet("ListAllServicePackage")]
        public IActionResult ListAllServicePackage()
        {
            var services = context.ServicePackages.
                Select(u => new ListServicePackageResponse
                {
                    PackageId = u.PackageId,
                    Name = u.Name,
                    Description = u.Description,
                    Price = u.Price,
                    DiscountPercentage = u.DiscountPercentage,
                    DurationInYears = u.DurationInYears,
                    MaxTours = u.MaxTours,
                    IsActive = u.IsActive,
                });

            if (!services.Any())
            {
                return NotFound("Not Found.");
            }

            return Ok(services);
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

            if (request.DurationInYears.HasValue)
                service.DurationInYears = request.DurationInYears.Value;

            if (request.MaxTours.HasValue)
                service.MaxTours = request.MaxTours.Value;

            if (request.IsActive.HasValue)
                service.IsActive = request.IsActive.Value;

            await context.SaveChangesAsync();

            return Ok(new { message = "Service package updated successfully." });
        }

        [HttpDelete("SoftDeleteServicePackage/{packageId}")]
        public async Task<IActionResult> SoftDeleteServicePackage(int packageId)
        {
            var service = await context.ServicePackages.FindAsync(packageId);
            if (service == null)
            {
                return NotFound("Service package not found.");
            }

            if (!service.IsActive)
            {
                return BadRequest("Service package is already inactive.");
            }

            service.IsActive = false;
            await context.SaveChangesAsync();

            return Ok(new { message = "Service package has been deactivated (soft deleted)." });
        }
    }
}
