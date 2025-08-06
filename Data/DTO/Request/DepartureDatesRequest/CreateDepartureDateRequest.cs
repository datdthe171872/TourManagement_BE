using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TourManagement_BE.Data.DTO.Request.DepartureDatesRequest;

public class CreateDepartureDateRequest
{
    [Required]
    public int TourId { get; set; }
    
    [Required]
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }
} 