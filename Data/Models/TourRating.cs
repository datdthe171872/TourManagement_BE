using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class TourRating
{
    public Guid RatingId { get; set; }

    public Guid TourId { get; set; }

    public Guid UserId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Tour Tour { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
