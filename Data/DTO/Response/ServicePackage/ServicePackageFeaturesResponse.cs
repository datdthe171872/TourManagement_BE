namespace TourManagement_BE.Data.DTO.Response.ServicePackage
{
    public class ServicePackageFeaturesResponse
    {
        public int FeatureId { get; set; }

        public int PackageId { get; set; }

        public string FeatureName { get; set; } = null!;

        public string FeatureValue { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
