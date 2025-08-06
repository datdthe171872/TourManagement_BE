using TourManagement_BE.Data.DTO.Request.DepartureDatesRequest;
using TourManagement_BE.Data.DTO.Response.DepartureDateResponse;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Service;

public interface IDepartureDateService
{
    Task<DepartureDate?> CreateDepartureDatesAsync(CreateDepartureDateRequest request, int userId);
    Task<List<DepartureDateResponse>> GetAllDepartureDatesAsync();
    Task<List<DepartureDateResponse>> GetDepartureDatesByTourIdAsync(int tourId);
    Task<List<DepartureDateWithBookingResponse>> GetDepartureDatesWithBookingsByTourOperatorAsync(int userId);
    Task<List<DepartureDateResponse>> GetDepartureDatesByTourOperatorAsync(int userId);
    Task<DepartureDateBookingsWrapperResponse?> GetBookingsByDepartureDateIdAsync(int departureDateId, int userId);
    Task<bool> CancelDepartureDateAsync(int departureDateId, int userId);
    Task<List<DepartureDateResponse>> GetCancelledDepartureDatesByTourOperatorAsync(int userId);
    Task<bool> ReactivateDepartureDateAsync(int departureDateId, int userId);
    Task<List<DepartureDateResponse>> GetDepartureDatesByTourGuideAsync(int userId);
} 