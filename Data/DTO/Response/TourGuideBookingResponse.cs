using System;

namespace TourManagement_BE.Data.DTO.Response
{
    public class TourGuideBookingResponse
    {
        public int BookingId { get; set; }
        public int TourId { get; set; }
        public int DepartureDateId { get; set; }
        public string TourTitle { get; set; }
        public DateTime DepartureDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public int NumberOfInfants { get; set; }
        public string NoteForTour { get; set; }
        public decimal TotalPrice { get; set; }
        public string BookingStatus { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? BookingDate { get; set; }
        public bool IsLeadGuide { get; set; }
        public DateTime? AssignedDate { get; set; }
    }
} 