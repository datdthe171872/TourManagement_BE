using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.DTO.Request
{
    public class CreateMultipleTourGuideAssignmentRequest
    {
        public int TourId { get; set; }
        public int DepartureDateId { get; set; }
        public List<TourGuideAssignmentItem> TourGuides { get; set; } = new List<TourGuideAssignmentItem>();
    }

    public class TourGuideAssignmentItem
    {
        public int TourGuideId { get; set; }
        public bool? IsLeadGuide { get; set; }
    }
} 