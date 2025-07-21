namespace TourManagement_BE.Data.DTO.Response.ServicePackage
{
    public class ListServicePackageResponse
    {
        public int PackageId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public int MaxTours { get; set; }

        public decimal TotalPrice { get; set; }

        public bool IsActive { get; set; }

    }
}
