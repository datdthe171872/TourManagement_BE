using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request.TourContract
{
    public class CreateContractRequest
    {
        [Required]
        public int BookingId { get; set; }

        public IFormFile? Contract { get; set; }
    }
}
