using TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Update;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Data.DTO.Request.TourRequest
{
    public class TourUpdateRequest
    {
        public int TourId { get; set; }
        
        //public int TourOperatorId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public decimal PriceOfAdults { get; set; }

        public decimal PriceOfChildren { get; set; }

        public decimal PriceOfInfants { get; set; }

        public string DurationInDays { get; set; } = null!;

        public string? StartPoint { get; set; }

        public string? Transportation { get; set; }

        public int MaxSlots { get; set; }

        public int MinSlots { get; set; }

        public string? Note { get; set; }

        public string? TourStatus { get; set; }

        public IFormFile? TourAvartarFile { get; set; }

        public bool IsActive { get; set; }

        public List<DepartureDateDto> ? DepartureDates { get; set; } = new();

        public List<TourExperienceDto> ? TourExperiences { get; set; } = new();

        public List<TourItineraryDto> ? TourItineraries { get; set; } = new();

        public List<TourMediaDto> ? TourMedia { get; set; } = new();

    }
}
