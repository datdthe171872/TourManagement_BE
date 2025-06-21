namespace TourManagement_BE.Data.DTO.Response;

public class TourOperatorResponse
{
    public int TourOperatorId { get; set; }
    public string? CompanyName { get; set; }
    public string? Description { get; set; }
    public string? CompanyLogo { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
}