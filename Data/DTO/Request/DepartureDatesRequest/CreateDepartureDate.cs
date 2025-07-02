namespace TourManagement_BE.Data.DTO.Request.DepartureDatesRequest
{
    public class CreateDepartureDate
    {
        public int TourId { get; set; }

        public DateTime DepartureDate1 { get; set; }

        //public bool IsActive { get; set; }
    }
}
