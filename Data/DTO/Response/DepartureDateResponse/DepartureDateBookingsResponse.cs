using System;
using System.Collections.Generic;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Data.DTO.Response.DepartureDateResponse
{
    public class DepartureDateBookingsResponse
    {
        public int DepartureDateId { get; set; }
        public string TourTitle { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public List<BookingDetailResponse> Bookings { get; set; } = new();
    }
} 