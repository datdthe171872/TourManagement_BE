using System;
using System.Collections.Generic;

namespace TourManagement_BE.Models;

public partial class TourItinerary
{
    public int ItineraryId { get; set; }

    public int TourId { get; set; }

    public int DayNumber { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ItineraryMedium> ItineraryMedia { get; set; } = new List<ItineraryMedium>();

    public virtual Tour Tour { get; set; } = null!;
}
