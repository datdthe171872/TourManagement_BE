using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request;

public class CreateFeedbackRequest
{
    [Required(ErrorMessage = "TourId là bắt buộc")]
    public int TourId { get; set; }

    [Required(ErrorMessage = "UserId là bắt buộc")]
    public int UserId { get; set; }

    [Range(1, 5, ErrorMessage = "Rating phải từ 1 đến 5")]
    public int? Rating { get; set; }

    [MaxLength(1000, ErrorMessage = "Comment không được vượt quá 1000 ký tự")]
    public string? Comment { get; set; }

    [MaxLength(500, ErrorMessage = "MediaUrl không được vượt quá 500 ký tự")]
    public string? MediaUrl { get; set; }
} 