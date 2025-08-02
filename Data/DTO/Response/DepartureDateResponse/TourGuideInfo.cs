namespace TourManagement_BE.Data.DTO.Response.DepartureDateResponse;

public class TourGuideInfo
{
    public int TourGuideId { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public int AssignmentId { get; set; }
    public DateOnly? AssignedDate { get; set; }
    public bool? IsLeadGuide { get; set; }
} 