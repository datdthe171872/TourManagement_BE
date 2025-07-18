using TourManagement_BE.Data;

namespace TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Update
{
    public class ItineraryMediaDto
    {
        public int MediaId { get; set; }

        public string? MediaUrl { get; set; } = null!;

        public IFormFile? MediaFile { get; set; }

        public string? MediaType { get; set; }

        public string? Caption { get; set; }

        public bool IsActive { get; set; }

    }
}
