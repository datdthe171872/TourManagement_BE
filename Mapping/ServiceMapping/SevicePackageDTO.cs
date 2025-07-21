using TourManagement_BE.Data.DTO.Request.ServicePackageRequest;
using TourManagement_BE.Data.Models;

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
                MaxTours = 10000,
                IsActive = u.IsActive,

            };
        }
    }
}
