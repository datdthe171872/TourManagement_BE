namespace TourManagement_BE.Data.DTO.Request;

public class TourGuideSearchRequest
{
    public string? Username { get; set; }
    public bool? IsActive { get; set; }
    public bool ShowInactive { get; set; } = false; // Default to false - hide inactive items
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
} 