using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TourManagement_BE.Data.DTO.Request
{
    public class CreateGuideNoteWithAttachmentRequest
    {
        [Required]
        public int BookingId { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        public List<IFormFile>? Attachments { get; set; }
    }
} 