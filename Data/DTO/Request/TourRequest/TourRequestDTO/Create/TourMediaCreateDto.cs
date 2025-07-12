using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Create
{
    public class TourMediaCreateDto
    {
        public IFormFile? MediaFile { get; set; }

        public string? MediaType { get; set; }

    }
}
