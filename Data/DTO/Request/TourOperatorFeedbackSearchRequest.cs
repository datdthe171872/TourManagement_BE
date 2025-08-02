using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request;

public class TourOperatorFeedbackSearchRequest
{
    public int? RatingId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
} 