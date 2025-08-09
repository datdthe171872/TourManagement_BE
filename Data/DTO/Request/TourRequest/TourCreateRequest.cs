using System.ComponentModel.DataAnnotations;
using TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Create;

namespace TourManagement_BE.Data.DTO.Request.TourRequest
{
    public class TourCreateRequest
    {
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public decimal PriceOfAdults { get; set; }

        public decimal PriceOfChildren { get; set; }

        public decimal PriceOfInfants { get; set; }

        public string DurationInDays { get; set; } = null!;

        public string? StartPoint { get; set; }

        public string? Transportation { get; set; }

        public int TourOperatorId { get; set; }

        public int MaxSlots { get; set; }

        public int MinSlots { get; set; }

        public string? Note { get; set; }

        //public string TourAvartar { get; set; } = null!;

        [Required]
        public IFormFile TourAvartarFile { get; set; } 


        public List<DepartureDateCreateDto> DepartureDates { get; set; } = new();
        public List<TourExperienceCreateDto> TourExperiences { get; set; } = new();
        public List<TourItineraryCreateDto> TourItineraries { get; set; } = new();
        public List<TourMediaCreateDto> TourMedia { get; set; } = new();
    }

}
