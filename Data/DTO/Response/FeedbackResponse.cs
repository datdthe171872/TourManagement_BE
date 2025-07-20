namespace TourManagement_BE.Data.DTO.Response;

public class FeedbackResponse
{
    public int RatingId { get; set; }
    public int TourId { get; set; }
    public int UserId { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public string? MediaUrl { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool IsActive { get; set; }
    
    // Additional info for display
    public string? TourName { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
} 