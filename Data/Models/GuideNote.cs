using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class GuideNote
{
    public int NoteId { get; set; }

    public int ReportId { get; set; }

    public int AssignmentId { get; set; }

    public int BookingId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public decimal? ExtraCost { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? DepartureDateId { get; set; }

    public bool IsActive { get; set; }

    public virtual TourGuideAssignment Assignment { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;

    public virtual ICollection<GuideNoteMedia> GuideNoteMedia { get; set; } = new List<GuideNoteMedia>();

    public virtual TourAcceptanceReport Report { get; set; } = null!;
}
