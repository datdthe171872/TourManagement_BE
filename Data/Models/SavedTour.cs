using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class SavedTour
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid TourId { get; set; }

    public DateTime? SavedAt { get; set; }

    public virtual Tour Tour { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
