using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Create
{
    public class TourMediaCreateDto
    {
        //public int Id { get; set; }
        //public string? MediaUrl { get; set; }

        public IFormFile? MediaFile { get; set; }

        public string? MediaType { get; set; }

        //public bool IsActive { get; set; }

    }
}
