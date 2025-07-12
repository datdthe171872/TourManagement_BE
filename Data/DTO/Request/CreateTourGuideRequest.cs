using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request
{
    public class CreateTourGuideRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Avatar { get; set; }
    }
} 