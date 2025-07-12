using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Create
{
    public class ItineraryMediaCreateDto
    {
        public IFormFile? MediaFile { get; set; }

        public string? MediaType { get; set; }

        public string? Caption { get; set; }

    }
}
