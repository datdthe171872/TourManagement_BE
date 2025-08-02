using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request;

public class UpdateFeedbackStatusRequest
{
    [Required(ErrorMessage = "RatingId là bắt buộc")]
    public int RatingId { get; set; }

    [Required(ErrorMessage = "IsActive là bắt buộc")]
    public bool IsActive { get; set; }
} 