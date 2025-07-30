namespace TourManagement_BE.Data.DTO.Request.ServicePackageRequest
{
    public class CreateServicePackageFeaturesRequest
    {
        public int NumberOfTours { get; set; }

        public int NumberOfTourAttribute { get; set; }

        public bool PostVideo { get; set; }
    }
}
