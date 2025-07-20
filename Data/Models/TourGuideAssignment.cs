using System;
using System.Collections.Generic;

namespace TourManagement_BE.Models;

public partial class TourGuideAssignment
{
    public int Id { get; set; }

    public int TourId { get; set; }

    public int BookingId { get; set; }

    public int TourGuideId { get; set; }

    public DateOnly? AssignedDate { get; set; }

    public int? NoteId { get; set; }

    public bool? IsLeadGuide { get; set; }

    public bool IsActive { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual ICollection<GuideNote> GuideNotes { get; set; } = new List<GuideNote>();

    public virtual ICollection<GuideRating> GuideRatings { get; set; } = new List<GuideRating>();

    public virtual TourGuide TourGuide { get; set; } = null!;
}
