using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class TourCancellation
{
    public Guid CancellationId { get; set; }

    public Guid TourId { get; set; }

    public Guid CancelledBy { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? Reason { get; set; }

    public virtual User CancelledByNavigation { get; set; } = null!;

    public virtual Tour Tour { get; set; } = null!;
}
