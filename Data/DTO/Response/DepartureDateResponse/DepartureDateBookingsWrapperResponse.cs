using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.DTO.Response.DepartureDateResponse
{
    public class DepartureDateBookingsWrapperResponse
    {
        public int DepartureDateId { get; set; }
        public string TourTitle { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public List<DepartureDateBookingDetailResponse> Bookings { get; set; } = new();
    }
}