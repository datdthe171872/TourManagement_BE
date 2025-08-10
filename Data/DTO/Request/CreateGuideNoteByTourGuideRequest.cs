using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request
{
    public class CreateGuideNoteByTourGuideRequest
    {
        [Required]
        public int BookingId { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        public decimal? ExtraCost { get; set; }
        
        public List<string>? AttachmentUrls { get; set; }
    }
} 