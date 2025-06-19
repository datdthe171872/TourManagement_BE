namespace TourManagement_BE.Data.DTO.Request.ProfileRequest
{
    public class UpdateProfileRequest
    {
        public int? UserId { get; set; } 
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? AvatarFile { get; set; }
    }

}
