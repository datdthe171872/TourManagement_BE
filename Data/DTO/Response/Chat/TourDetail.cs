using System;
using System.Collections.Generic;

namespace TourManagement_BE.Data.DTO.Response.Chat
{
	public class TourDetail
	{
		public int TourId { get; set; }
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public decimal PriceOfAdults { get; set; }
		public string? DurationInDays { get; set; }
		public string? StartPoint { get; set; }
		public string? Transportation { get; set; }
		public string? TourAvartar { get; set; }
		public List<DateTime> UpcomingDepartureDates { get; set; } = new();
	}
}


