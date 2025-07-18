using TourManagement_BE.Data.DTO.Request.ServicePackageRequest;
using TourManagement_BE.Models;

namespace TourManagement_BE.Mapping.ServiceMapping
{
    public static class SevicePackageDTO
    {
        public static ServicePackage? ToDto(this CreateServicePackageRequest u)
        {
            return new ServicePackage
            {
                Name = u.Name,
                Description = u.Description,
                Price = u.Price,
                DiscountPercentage = u.DiscountPercentage,
                DurationInYears = u.DurationInYears,
                MaxTours = u.MaxTours,
                IsActive = u.IsActive,

            };
        }
    }
}
