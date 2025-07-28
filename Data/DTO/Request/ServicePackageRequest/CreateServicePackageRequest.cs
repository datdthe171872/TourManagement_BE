using TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Create;

namespace TourManagement_BE.Data.DTO.Request.ServicePackageRequest
{
    public class CreateServicePackageRequest
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int DurationInDay { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public List<CreateServicePackageFeaturesRequest> ServicePackageFeaturesRequests { get; set; } = new();
    }
}
