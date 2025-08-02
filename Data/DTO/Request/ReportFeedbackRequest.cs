using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request;

public class ReportFeedbackRequest
{
    [Required(ErrorMessage = "RatingId là bắt buộc")]
    public int RatingId { get; set; }
    
    [Required(ErrorMessage = "Lý do report là bắt buộc")]
    [MaxLength(1000, ErrorMessage = "Lý do report không được vượt quá 1000 ký tự")]
    public string Reason { get; set; } = string.Empty;
} 