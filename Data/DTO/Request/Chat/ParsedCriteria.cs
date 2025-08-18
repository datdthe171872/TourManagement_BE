using System;

namespace TourManagement_BE.Data.DTO.Request.Chat
{
	public class ParsedCriteria
	{
		public string? Destination { get; set; }
		public DateTime? DateFrom { get; set; }
		public DateTime? DateTo { get; set; }
		public int? DurationDays { get; set; }
		public int? GroupSize { get; set; }
		public decimal? BudgetMin { get; set; }
		public decimal? BudgetMax { get; set; }
		public string? Style { get; set; }
		public string? StartPoint { get; set; }
		public string? Transportation { get; set; }
	}
}


