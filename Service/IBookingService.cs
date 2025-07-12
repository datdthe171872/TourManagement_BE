using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service
{
    public interface IBookingService
    {
        Task<BookingListResponse> GetBookingsAsync(BookingSearchRequest request);
        Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request);
        Task<BookingResponse> UpdateBookingAsync(UpdateBookingRequest request);
        Task<bool> SoftDeleteBookingAsync(int bookingId, int userId);
    }
} 