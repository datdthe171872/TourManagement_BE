using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
