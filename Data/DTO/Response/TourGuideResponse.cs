namespace TourManagement_BE.Data.DTO.Response;

public class TourGuideResponse
{
    public int TourGuideId { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Avatar { get; set; }
    public bool IsActive { get; set; }
    public int? TourOperatorId { get; set; }
} 