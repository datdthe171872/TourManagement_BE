using System;
using System.Collections.Generic;

namespace TourManagement_BE.Models;

public partial class ItineraryMedia
{
    public int MediaId { get; set; }

    public int ItineraryId { get; set; }

    public string MediaUrl { get; set; } = null!;

    public string? MediaType { get; set; }

    public string? Caption { get; set; }

    public DateTime? UploadedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual TourItinerary Itinerary { get; set; } = null!;
}
