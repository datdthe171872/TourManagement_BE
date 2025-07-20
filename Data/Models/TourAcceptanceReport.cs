using System;
using System.Collections.Generic;

namespace TourManagement_BE.Models;

public partial class TourAcceptanceReport
{
    public int ReportId { get; set; }

    public int BookingId { get; set; }

    public int TourGuideId { get; set; }

    public DateTime? ReportDate { get; set; }

    public string? Summary { get; set; }

    public decimal? TotalExtraCost { get; set; }

    public string? Notes { get; set; }

    public string? AttachmentUrl { get; set; }

    public bool IsActive { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual TourGuide TourGuide { get; set; } = null!;
}
