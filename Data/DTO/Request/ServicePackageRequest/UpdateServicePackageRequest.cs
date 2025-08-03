namespace TourManagement_BE.Data.DTO.Request.ServicePackageRequest
{
    public class UpdateServicePackageRequest
    {
        public int PackageId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public int MaxTour { get; set; }

        public int MaxImage { get; set; }

        public bool MaxVideo { get; set; }

        public bool TourGuideFunction { get; set; }

        public bool IsActive { get; set; }
    }
}
