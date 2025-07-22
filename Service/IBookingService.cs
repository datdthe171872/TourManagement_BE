using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service
{
    public interface IBookingService
    {
        Task<BookingListResponse> GetBookingsAsync(BookingSearchRequest request);
        Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request, int userId);
        Task<BookingResponse> UpdateBookingAsync(UpdateBookingRequest request);
        Task<bool> SoftDeleteBookingAsync(int bookingId, int userId);
        
        // New methods for role-based booking retrieval
        Task<BookingListResponse> GetCustomerBookingsAsync(BookingSearchRequest request, int userId);
        Task<BookingListResponse> GetTourOperatorBookingsAsync(BookingSearchRequest request);
        Task<BookingListResponse> GetAllBookingsForAdminAsync(BookingSearchRequest request);
        Task<BookingResponse> GetBookingByIdAsync(int bookingId);
        Task<BookingResponse> UpdateBookingContractAsync(UpdateBookingRequest request);
    }
} 