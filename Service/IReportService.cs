using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service
{
    public interface IReportService
    {
        Task<List<ReportResponse>> GetReportsForCustomerAsync(int userId, string? username = null);
        Task<List<ReportResponse>> GetReportsForOperatorAsync(int tourOperatorId, string? username = null);
    }
} 