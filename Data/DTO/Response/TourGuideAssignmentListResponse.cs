using System.Collections.Generic;

namespace TourManagement_BE.Data.DTO.Response
{
    public class TourGuideAssignmentListResponse
    {
        public List<TourGuideAssignmentResponse> Assignments { get; set; } = new List<TourGuideAssignmentResponse>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
} 