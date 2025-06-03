using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.Models;

public partial class Tour
{
    public Guid TourId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string DurationInDays { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string? StartPoint { get; set; }

    public string? Transportation { get; set; }

    public Guid TourOperatorId { get; set; }

    public int MaxSlots { get; set; }

    public int? SlotsBooked { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Note { get; set; }

    public string? TourStatus { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<DepartureDate> DepartureDates { get; set; } = new List<DepartureDate>();

    public virtual ICollection<SavedTour> SavedTours { get; set; } = new List<SavedTour>();

    public virtual ICollection<TourCancellation> TourCancellations { get; set; } = new List<TourCancellation>();

    public virtual ICollection<TourExperience> TourExperiences { get; set; } = new List<TourExperience>();

    public virtual ICollection<TourGuideAssignment> TourGuideAssignments { get; set; } = new List<TourGuideAssignment>();

    public virtual ICollection<TourImage> TourImages { get; set; } = new List<TourImage>();

    public virtual TourOperator TourOperator { get; set; } = null!;

    public virtual ICollection<TourRating> TourRatings { get; set; } = new List<TourRating>();
}
