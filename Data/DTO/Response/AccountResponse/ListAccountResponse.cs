namespace TourManagement_BE.Data.DTO.Response.AccountResponse
{
    public class ListAccountResponse
    {
        public int? UserId { get; set; }
        public string? UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public string RoleName { get; set; } = null!;
        public bool? IsActive { get; set; }
    }
}
