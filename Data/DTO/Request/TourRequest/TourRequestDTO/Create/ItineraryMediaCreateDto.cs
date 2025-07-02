using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Create
{
    public class ItineraryMediaCreateDto
    {
        //public int MediaId { get; set; }
        //public string MediaUrl { get; set; } = null!;

        public IFormFile? MediaFile { get; set; }

        public string? MediaType { get; set; }

        public string? Caption { get; set; }

        //public DateTime? UploadedAt { get; set; }

        //public bool IsActive { get; set; }

    }
}
