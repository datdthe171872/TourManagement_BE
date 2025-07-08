using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request
{
    public class CreateGuideNoteRequest
    {
        [Required]
        public int AssignmentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> MediaUrls { get; set; }
    }
} 