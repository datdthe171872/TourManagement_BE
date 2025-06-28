namespace TourManagement_BE.Data.DTO.Request;

public class FeedbackSearchRequest
{
    public int? TourId { get; set; }
    public int? UserId { get; set; }
    public int? Rating { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
} 