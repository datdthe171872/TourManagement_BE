using TourManagement_BE.Data;

namespace TourManagement_BE.Data.DTO.Response.TourResponse.TourDetailDTO
{
    public class ItineraryMediaDto
    {
        public int MediaId { get; set; }

        public string MediaUrl { get; set; } = null!;

        public string? MediaType { get; set; }

        public string? Caption { get; set; }

        public DateTime? UploadedAt { get; set; }

        public bool IsActive { get; set; }

    }
}
