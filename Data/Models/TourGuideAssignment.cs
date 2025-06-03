using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class TourGuideAssignment
{
    public Guid Id { get; set; }

    public Guid TourId { get; set; }

    public Guid TourGuideId { get; set; }

    public DateOnly? AssignedDate { get; set; }

    public string? Notes { get; set; }

    public bool? IsLeadGuide { get; set; }

    public virtual Tour Tour { get; set; } = null!;

    public virtual TourGuide TourGuide { get; set; } = null!;
}
