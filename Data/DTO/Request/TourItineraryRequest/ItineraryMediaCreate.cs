namespace TourManagement_BE.Data.DTO.Request.TourItineraryRequest
{
    public class ItineraryMediaCreate
    {
        public int ItineraryId { get; set; }
        public IFormFile MediaFile { get; set; } = null!;
        public string MediaType { get; set; } = null!;
        public string? Caption { get; set; }
    }
}
