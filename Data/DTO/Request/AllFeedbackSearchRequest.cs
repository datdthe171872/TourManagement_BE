namespace TourManagement_BE.Data.DTO.Request;

public class AllFeedbackSearchRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? TourId { get; set; }
    public int? UserId { get; set; }
    public int? Rating { get; set; }
    public bool? IsActive { get; set; }
} 