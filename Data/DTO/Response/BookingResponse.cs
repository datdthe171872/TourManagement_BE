using System;

namespace TourManagement_BE.Data.DTO.Response
{
    public class BookingResponse
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int TourId { get; set; }
        public int DepartureDateId { get; set; }
        public DateTime? BookingDate { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public int NumberOfInfants { get; set; }
        public string? NoteForTour { get; set; }
        public decimal? TotalPrice { get; set; }       
        public string? Contract { get; set; }
        public string? BookingStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public bool IsActive { get; set; }
        public string? UserName { get; set; }
        public string? TourTitle { get; set; }
        public string? CompanyName { get; set; }
        public int? TourOperatorId { get; set; }
    }
} 