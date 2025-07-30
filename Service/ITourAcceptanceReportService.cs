using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service
{
    public interface ITourAcceptanceReportService
    {
        Task<List<TourAcceptanceReportResponse>> GetReportsByGuideUserIdAsync(int userId);
        Task<TourAcceptanceReportResponse> GetReportByIdAsync(int reportId, int userId);
        Task<TourAcceptanceReportResponse> CreateReportAsync(int userId, CreateTourAcceptanceReportRequest request);
        Task<TourAcceptanceReportResponse> UpdateReportAsync(int userId, int reportId, UpdateTourAcceptanceReportRequest request);
        Task<bool> DeleteReportAsync(int userId, int reportId);
        Task<List<TourAcceptanceReportResponse>> GetReportsByBookingIdAsync(int bookingId);
    }
} 