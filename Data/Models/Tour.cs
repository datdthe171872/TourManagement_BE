using System;
using System.Collections.Generic;

namespace TourManagement_BE.Models;

public partial class Tour
{
    public int TourId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string DurationInDays { get; set; } = null!;

    public string? StartPoint { get; set; }

    public string? Transportation { get; set; }

    public int TourOperatorId { get; set; }

    public int MaxSlots { get; set; }

    public int? SlotsBooked { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string TourType { get; set; } = null!;

    public string? Note { get; set; }

    public string? TourStatus { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<DepartureDate> DepartureDates { get; set; } = new List<DepartureDate>();

    public virtual ICollection<SavedTour> SavedTours { get; set; } = new List<SavedTour>();

    public virtual ICollection<TourCancellation> TourCancellations { get; set; } = new List<TourCancellation>();

    public virtual ICollection<TourExperience> TourExperiences { get; set; } = new List<TourExperience>();

    public virtual ICollection<TourItinerary> TourItineraries { get; set; } = new List<TourItinerary>();

    public virtual ICollection<TourMedia> TourMedia { get; set; } = new List<TourMedia>();

    public virtual TourOperator TourOperator { get; set; } = null!;

    public virtual ICollection<TourRating> TourRatings { get; set; } = new List<TourRating>();
}
