using TourManagement_BE.Models;

namespace TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Update
{
    public class TourMediaDto
    {
        public int Id { get; set; }

        public string? MediaUrl { get; set; } 

        public IFormFile? MediaFile { get; set; } 

        public string? MediaType { get; set; }

        public bool IsActive { get; set; }
    }

}
