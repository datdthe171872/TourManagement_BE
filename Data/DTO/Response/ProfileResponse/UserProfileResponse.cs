namespace TourManagement_BE.Data.DTO.Response.ProfileResponse
{
    public class UserProfileResponse
    {
        public int? UserId { get; set; }
        public string? UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public string RoleName { get; set; } = null!;
        public bool? IsActive { get; set; }

        public string? CompanyName { get; set; }
        public string? Description { get; set; }

        public int? HomeTourGuideId { get; set; }
    }

}
