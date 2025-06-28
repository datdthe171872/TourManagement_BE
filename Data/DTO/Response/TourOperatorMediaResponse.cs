namespace TourManagement_BE.Data.DTO.Response;

public class TourOperatorMediaResponse
{
    public int Id { get; set; }
    public int TourOperatorId { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime? UploadedAt { get; set; }
    public bool IsActive { get; set; }
} 