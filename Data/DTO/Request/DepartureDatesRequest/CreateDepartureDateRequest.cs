using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request.DepartureDatesRequest;

public class CreateDepartureDateRequest
{
    [Required]
    public int TourId { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
} 