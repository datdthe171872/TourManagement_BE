using System;

namespace TourManagement_BE.Data.DTO.Request
{
    public class TourGuideAssignmentSearchRequest
    {
        public int? TourId { get; set; }
        public int? TourGuideId { get; set; }
        public int? BookingId { get; set; }
        public bool? IsLeadGuide { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? AssignedDateFrom { get; set; }
        public DateTime? AssignedDateTo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
} 