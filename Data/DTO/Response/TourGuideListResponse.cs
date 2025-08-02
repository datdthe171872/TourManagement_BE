namespace TourManagement_BE.Data.DTO.Response;

public class TourGuideListResponse
{
    public List<TourGuideResponse> TourGuides { get; set; } = new List<TourGuideResponse>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
} 