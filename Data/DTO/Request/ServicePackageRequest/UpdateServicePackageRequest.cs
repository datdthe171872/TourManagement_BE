namespace TourManagement_BE.Data.DTO.Request.ServicePackageRequest
{
    public class UpdateServicePackageRequest
    {
        public int PackageId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public bool? IsActive { get; set; }
    }
}
