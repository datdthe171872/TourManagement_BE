using System.Collections.Generic;

namespace TourManagement_BE.Data.DTO.Response.Chat
{
	public class ChatResponse
	{
		public string Reply { get; set; } = string.Empty;
		public List<TourSuggestionItem> Suggestions { get; set; } = new();
		public string Intent { get; set; } = "suggest";
		public object? Payload { get; set; }
	}

	public class TourSuggestionItem
	{
		public int TourId { get; set; }
		public string Title { get; set; } = string.Empty;
		public decimal PriceOfAdults { get; set; }
		public decimal PriceOfChildren { get; set; }
		public decimal PriceOfInfants { get; set; }
		public string? DurationInDays { get; set; }
		public string? StartPoint { get; set; }
		public string? TourAvartar { get; set; }
		public List<System.DateTime> UpcomingDepartureDates { get; set; } = new();
	}
}


