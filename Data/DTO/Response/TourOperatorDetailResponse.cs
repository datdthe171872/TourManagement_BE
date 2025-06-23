namespace TourManagement_BE.Data.DTO.Response;

public class TourOperatorDetailResponse
{
    public int TourOperatorId { get; set; }
    public int? UserId { get; set; }
    public string? CompanyName { get; set; }
    public string? Description { get; set; }
    public string? CompanyLogo { get; set; }
    public string? LicenseNumber { get; set; }
    public DateOnly? LicenseIssuedDate { get; set; }
    public string? TaxCode { get; set; }
    public int? EstablishedYear { get; set; }
    public string? Hotline { get; set; }
    public string? Website { get; set; }
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public string? Address { get; set; }
    public string? WorkingHours { get; set; }
    public bool IsActive { get; set; }
}