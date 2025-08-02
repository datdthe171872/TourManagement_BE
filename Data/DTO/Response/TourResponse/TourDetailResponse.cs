using TourManagement_BE.Data.DTO.Response.TourResponse.TourDetailDTO;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Data.DTO.Response.TourResponse
{
    public class TourDetailResponse
    {
        public int TourId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public decimal PriceOfAdults { get; set; }

        public decimal PriceOfChildren { get; set; }

        public decimal PriceOfInfants { get; set; }

        public string DurationInDays { get; set; } = null!;

        public string? StartPoint { get; set; }

        public string? Transportation { get; set; }

        public double? AverageRating { get; set; }

        public int TourOperatorId { get; set; }

        public string? TourOperatorName { get; set; }

        public string? CompanyName { get; set; }

        public string? CompanyDescription { get; set; }

        public string? CompanyLogo { get; set; }

        public string? CompanyHotline { get; set; }

        public int MaxSlots { get; set; }

        public int MinSlots { get; set; }

        public int? SlotsBooked { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? Note { get; set; }

        public string? TourStatus { get; set; }

        public string TourAvartar { get; set; }

        public bool IsActive { get; set; }

        public List<DepartureDateDto> DepartureDates { get; set; } = new();

        public List<TourExperienceDto> TourExperiences { get; set; } = new();

        public List<TourItineraryDto> TourItineraries { get; set; } = new();

        public List<TourMediaDto> TourMedia { get; set; } = new();

        public List<TourRatingDto> TourRatings { get; set; } = new();
    }
}
