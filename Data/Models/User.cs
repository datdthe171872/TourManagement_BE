using System;
using System.Collections.Generic;

namespace TourManagement_BE.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Avatar { get; set; }

    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<GuideRating> GuideRatings { get; set; } = new List<GuideRating>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<SavedTour> SavedTours { get; set; } = new List<SavedTour>();

    public virtual ICollection<TourCancellation> TourCancellations { get; set; } = new List<TourCancellation>();

    public virtual TourGuide? TourGuide { get; set; }

    public virtual TourOperator? TourOperator { get; set; }

    public virtual ICollection<TourRating> TourRatings { get; set; } = new List<TourRating>();
}
