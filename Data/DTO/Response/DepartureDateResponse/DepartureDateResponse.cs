using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.DTO.Response.DepartureDateResponse;

public class DepartureDateResponse
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public string TourTitle { get; set; } = null!;
    public DateTime DepartureDate { get; set; }
    public bool IsActive { get; set; }
    public int TotalBookings { get; set; }
    public int AvailableSlots { get; set; }
    public List<TourGuideInfo> TourGuides { get; set; } = new List<TourGuideInfo>();
} 