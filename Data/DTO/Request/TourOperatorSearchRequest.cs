using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request;

public class TourOperatorSearchRequest
{
    [StringLength(100, ErrorMessage = "Tên công ty không được vượt quá 100 ký tự")]
    public string? CompanyName { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}