using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.DTO.Response
{
    public class GuideNoteResponse
    {
        public int NoteId { get; set; }
        public int AssignmentId { get; set; }
        public int ReportId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public decimal? ExtraCost { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<string> MediaUrls { get; set; }
    }
} 