using System;
using System.Collections.Generic;

namespace TourManagement_BE.Models;

public partial class TourCancellation
{
    public int CancellationId { get; set; }

    public int TourId { get; set; }

    public int CancelledBy { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? Reason { get; set; }

    public bool IsActive { get; set; }

    public virtual User CancelledByNavigation { get; set; } = null!;

    public virtual Tour Tour { get; set; } = null!;
}
