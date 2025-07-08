using System;

namespace TourManagement_BE.Data.DTO.Request
{
    public class CreateTourGuideAssignmentRequest
    {
        public int TourId { get; set; }
        public int BookingId { get; set; }
        public int TourGuideId { get; set; }
        public DateTime? AssignedDate { get; set; }
        public bool? IsLeadGuide { get; set; }
    }
} 