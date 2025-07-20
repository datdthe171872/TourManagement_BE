using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class SavedTour
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TourId { get; set; }

    public DateTime? SavedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Tour Tour { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
