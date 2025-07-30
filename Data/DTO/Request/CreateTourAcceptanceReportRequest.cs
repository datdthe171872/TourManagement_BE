using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request
{
    public class CreateTourAcceptanceReportRequest
    {
        [Required]
        public int BookingId { get; set; }
        
        [Required]
        public int AssignmentId { get; set; }
        
        public string? Summary { get; set; }
        
        public decimal? TotalExtraCost { get; set; }
        
        public string? Notes { get; set; }
        
        public string? AttachmentUrl { get; set; }
    }
} 