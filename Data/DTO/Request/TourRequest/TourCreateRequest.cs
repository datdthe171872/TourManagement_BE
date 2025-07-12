using TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Create;

namespace TourManagement_BE.Data.DTO.Request.TourRequest
{
    public class TourCreateRequest
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string DurationInDays { get; set; } = null!;
        public string? StartPoint { get; set; }
        public string? Transportation { get; set; }
        public int TourOperatorId { get; set; }
        public int MaxSlots { get; set; }
        public string TourType { get; set; } = null!;
        public string? Note { get; set; }
        public string? TourStatus { get; set; }

        public List<DepartureDateCreateDto> DepartureDates { get; set; } = new();
        public List<TourExperienceCreateDto> TourExperiences { get; set; } = new();
        public List<TourItineraryCreateDto> TourItineraries { get; set; } = new();
        public List<TourMediaCreateDto> TourMedia { get; set; } = new();
    }

}
