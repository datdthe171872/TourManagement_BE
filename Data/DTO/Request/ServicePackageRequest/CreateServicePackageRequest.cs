namespace TourManagement_BE.Data.DTO.Request.ServicePackageRequest
{
    public class CreateServicePackageRequest
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public bool IsActive { get; set; }
    }
}
