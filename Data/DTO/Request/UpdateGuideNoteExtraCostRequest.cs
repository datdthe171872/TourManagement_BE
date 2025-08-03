using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request
{
    public class UpdateGuideNoteExtraCostRequest
    {
        [Required]
        public decimal ExtraCost { get; set; }
    }
} 