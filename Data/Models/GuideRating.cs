using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class GuideRating
{
    public int RatingId { get; set; }

    public int UserId { get; set; }

    public int TourGuideId { get; set; }

    public int AssignmentId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public string? MediaUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual TourGuideAssignment Assignment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
