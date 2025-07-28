namespace TourManagement_BE.Data.DTO.Request.ServicePackageRequest
{
    public class CreateServicePackageFeaturesRequest
    {
        public string FeatureName { get; set; } = null!;

        public string FeatureValue { get; set; } = null!;
    }
}
