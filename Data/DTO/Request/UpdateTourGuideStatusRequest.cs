using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request
{
    public class UpdateTourGuideStatusRequest
    {
        [Required]
        public int TourGuideId { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
} 