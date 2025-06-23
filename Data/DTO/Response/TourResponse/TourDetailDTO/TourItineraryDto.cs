using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Data.DTO.Response.TourResponse.TourDetailDTO
{
    public class TourItineraryDto
    {
        public int ItineraryId { get; set; }

        public int DayNumber { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public List<ItineraryMediaDto> ItineraryMedia { get; set; } = new();

    }
}
