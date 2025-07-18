using TourManagement_BE.Data;

namespace TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Update
{
    public class TourItineraryDto
    {
        public int ItineraryId { get; set; }
        public int DayNumber { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public List<ItineraryMediaDto> ItineraryMedia { get; set; } = new();

    }
}
