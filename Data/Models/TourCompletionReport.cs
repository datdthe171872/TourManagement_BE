using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class TourCompletionReport
{
    public Guid ReportId { get; set; }

    public Guid BookingId { get; set; }

    public Guid TourGuideId { get; set; }

    public DateTime? ReportDate { get; set; }

    public string? Summary { get; set; }

    public decimal? TotalExtraCost { get; set; }

    public string? Notes { get; set; }

    public string? AttachmentUrl { get; set; }

    public string? AttachmentType { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual TourGuide TourGuide { get; set; } = null!;
}
