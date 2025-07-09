using System;

namespace TourManagement_BE.Data.DTO.Response
{
    public class TourGuideAssignmentResponse
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int BookingId { get; set; }
        public int TourGuideId { get; set; }
        public DateOnly? AssignedDate { get; set; }
        public int? NoteId { get; set; }
        public bool? IsLeadGuide { get; set; }
        public bool IsActive { get; set; }
        
        // Thông tin bổ sung từ các entity liên quan
        public string? TourName { get; set; }
        public string? GuideName { get; set; }
        public string? CustomerName { get; set; }
    }
} 