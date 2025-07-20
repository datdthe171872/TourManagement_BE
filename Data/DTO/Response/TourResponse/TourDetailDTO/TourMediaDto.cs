using TourManagement_BE.Models;

namespace TourManagement_BE.Data.DTO.Response.TourResponse.TourDetailDTO
{
    public class TourMediaDto
    {
        public int Id { get; set; }

        public string? MediaUrl { get; set; }

        public string? MediaType { get; set; }

        public bool IsActive { get; set; }

    }
}
