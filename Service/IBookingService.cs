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
        Task<BookingListResponse> GetTourOperatorBookingsAsync(BookingSearchRequest request, int userId);
        Task<BookingListResponse> GetAllBookingsForAdminAsync(BookingSearchRequest request);
        Task<BookingResponse> GetBookingByIdAsync(int bookingId);
        Task<BookingDetailResponse> GetBookingByIdDetailedAsync(int bookingId);
        Task<BookingResponse> UpdateBookingContractAsync(UpdateBookingRequest request);
        
        // New detailed booking methods
        Task<BookingDetailListResponse> GetBookingsDetailedAsync(BookingSearchRequest request);
        Task<BookingDetailListResponse> GetCustomerBookingsDetailedAsync(BookingSearchRequest request, int userId);
        Task<BookingDetailListResponse> GetTourOperatorBookingsDetailedAsync(BookingSearchRequest request, int userId);
        Task<BookingDetailListResponse> GetAllBookingsForAdminDetailedAsync(BookingSearchRequest request);
        
        // Status update methods for Tour Operator
        Task<BookingResponse> UpdatePaymentStatusAsync(UpdatePaymentStatusRequest request, int tourOperatorId);
        Task<BookingResponse> UpdateBookingStatusAsync(UpdateBookingStatusRequest request, int tourOperatorId);
        
        // Cancel booking method for Customer
        Task<BookingResponse> CancelBookingAsync(int bookingId, int userId);
        
        // Toggle booking visibility for Tour Operator
        Task<BookingResponse> ToggleBookingVisibilityAsync(int bookingId, int tourOperatorId);

        // Update payment information
        Task<BookingResponse> UpdateBookingPaymentAsync(UpdateBookingPaymentRequest request, int userId);
    }
} 