using System.Collections.Generic;

namespace TourManagement_BE.Data.DTO.Response
{
    public class BookingDetailListResponse
    {
        public List<BookingDetailResponse> Bookings { get; set; } = new List<BookingDetailResponse>();
    }
} 