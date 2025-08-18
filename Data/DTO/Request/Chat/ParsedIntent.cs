namespace TourManagement_BE.Data.DTO.Request.Chat
{
	public class ParsedIntent
	{
		// intent: suggest | tour_detail | compare_tours | plan_booking | booking_status | help
		public string Intent { get; set; } = "suggest";
		public int? TourId { get; set; }
		public int? Tour1Id { get; set; }
		public int? Tour2Id { get; set; }
		public int? BookingId { get; set; }
		public BookingPlan? Plan { get; set; }
		// tour_status
		public string? TourTitle { get; set; }
		public int? TourOperatorId { get; set; }
		public string? TourOperatorName { get; set; }
	}
}


