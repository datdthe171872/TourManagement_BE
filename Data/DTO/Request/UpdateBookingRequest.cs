using System;

namespace TourManagement_BE.Data.DTO.Request
{
    public class UpdateBookingCustomerRequest
    {
        public int BookingId { get; set; }
        public int DepartureDateId { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public int NumberOfInfants { get; set; }
        public string? NoteForTour { get; set; }
    }

    public class UpdateBookingOperatorRequest
    {
        public int BookingId { get; set; }
        public string? Contract { get; set; }
    }

    // Giữ lại cho service nội bộ nếu cần
    public class UpdateBookingRequest
    {
        public int BookingId { get; set; }
        public int DepartureDateId { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public int NumberOfInfants { get; set; }
        public string? NoteForTour { get; set; }
        public string? Contract { get; set; }
    }
} 