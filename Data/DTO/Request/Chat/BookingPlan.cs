namespace TourManagement_BE.Data.DTO.Request.Chat
{
	public class BookingPlan
	{
		public int? TourId { get; set; }
		public int? DepartureDateId { get; set; }
		public int? NumberOfAdults { get; set; }
		public int? NumberOfChildren { get; set; }
		public int? NumberOfInfants { get; set; }
		public bool Confirm { get; set; } = false;
	}
}


