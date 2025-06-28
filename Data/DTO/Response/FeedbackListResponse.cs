namespace TourManagement_BE.Data.DTO.Response;

public class FeedbackListResponse
{
    public List<FeedbackResponse> Feedbacks { get; set; } = new List<FeedbackResponse>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
} 