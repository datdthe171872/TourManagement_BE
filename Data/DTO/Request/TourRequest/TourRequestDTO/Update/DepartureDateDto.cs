using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Update
{
    public class DepartureDateDto
    {
        public int Id { get; set; }

        public DateTime DepartureDate1 { get; set; }

        public bool IsActive { get; set; }

    }
}
