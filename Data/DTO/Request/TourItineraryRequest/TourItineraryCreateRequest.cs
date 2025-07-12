namespace TourManagement_BE.Data.DTO.Request.TourItineraryRequest
{
    public class TourItineraryCreateRequest
    {
        public int TourId { get; set; }
        //public int DayNumber { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }

}
