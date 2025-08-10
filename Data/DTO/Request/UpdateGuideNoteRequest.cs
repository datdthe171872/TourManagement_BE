using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request
{
    public class UpdateGuideNoteRequest
    {
        [Required]
        public string Title { get; set; }
        public string Content { get; set; }
        public decimal? ExtraCost { get; set; }
        public List<string> MediaUrls { get; set; }
    }
} 