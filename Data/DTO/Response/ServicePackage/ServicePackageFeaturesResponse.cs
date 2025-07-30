namespace TourManagement_BE.Data.DTO.Response.ServicePackage
{
    public class ServicePackageFeaturesResponse
    {
        public int FeatureId { get; set; }

        public int PackageId { get; set; }

        public int NumberOfTours { get; set; }

        public int NumberOfTourAttribute { get; set; }

        public bool PostVideo { get; set; }

        public bool IsActive { get; set; }
    }
}
