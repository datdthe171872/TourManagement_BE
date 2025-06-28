using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
    }
} 