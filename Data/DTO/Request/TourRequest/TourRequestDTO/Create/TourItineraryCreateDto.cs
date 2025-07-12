using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Create
{
    public class TourItineraryCreateDto
    {
        public int DayNumber { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public List<ItineraryMediaCreateDto> ItineraryMedia { get; set; } = new();

    }
}
