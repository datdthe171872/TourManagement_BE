using TourManagement_BE.Data.DTO.Request.ServicePackageRequest;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Mapping.ServiceMapping
{
    public static class SevicePackageDTO
    {
        public static ServicePackage? ToDto(this CreateServicePackageRequest u)
        {
            var servicePackage = new ServicePackage
            {
                Name = u.Name,
                Description = u.Description,
                Price = u.Price,
                DiscountPercentage = u.DiscountPercentage,
                MaxTour = u.MaxTour,
                MaxImage = u.MaxImage,
                MaxVideo = u.MaxVideo,
                TourGuideFunction = u.TourGuideFunction,
                IsActive = true,
            };
            return servicePackage;
        }
    }
}
