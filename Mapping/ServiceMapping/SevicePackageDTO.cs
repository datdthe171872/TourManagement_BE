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
                DurationInDay = u.DurationInDay, 
                DiscountPercentage = u.DiscountPercentage,
                IsActive = true,
                ServicePackageFeatures = new List<ServicePackageFeature>()
            };

            foreach (var featureRequest in u.ServicePackageFeaturesRequests)
            {
                servicePackage.ServicePackageFeatures.Add(new ServicePackageFeature
                {
                    FeatureName = featureRequest.FeatureName,
                    FeatureValue = featureRequest.FeatureValue,
                    IsActive = true,
                    Package = servicePackage 
                });
            }

            return servicePackage;
        }
    }
}
