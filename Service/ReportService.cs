using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service
{
    public class ReportService : IReportService
    {
        private readonly MyDBContext _context;

        public ReportService(MyDBContext context)
        {
            _context = context;
        }

        public async Task<List<ReportResponse>> GetReportsForCustomerAsync(int userId, string? username = null)
        {
            var query = _context.Bookings
                .Where(b => b.UserId == userId && b.IsActive &&
                    (string.IsNullOrWhiteSpace(username) || (b.User.UserName != null && b.User.UserName.Contains(username))))
                .Include(b => b.User)
                .Include(b => b.Tour)
                .Include(b => b.TourAcceptanceReports);

            return await query
                .Select(b => new ReportResponse
                {
                    Username = b.User.UserName ?? "Unknown",
                    BookingId = b.BookingId,
                    TourTitle = b.Tour.Title ?? "Unknown Tour",
                    Contract = b.Contract ?? "N/A",
                    TotalPrice = b.TotalPrice ?? 0,
                    TotalExtraCost = b.TourAcceptanceReports
                        .Where(r => r.IsActive)
                        .Sum(r => r.TotalExtraCost) ?? 0,
                    Total = (b.TotalPrice ?? 0) + (b.TourAcceptanceReports
                        .Where(r => r.IsActive)
                        .Sum(r => r.TotalExtraCost) ?? 0)
                })
                .ToListAsync();
        }

        public async Task<List<ReportResponse>> GetReportsForOperatorAsync(int tourOperatorId, string? username = null)
        {
            var query = _context.Bookings
                .Where(b => b.Tour.TourOperatorId == tourOperatorId && b.IsActive &&
                    (string.IsNullOrWhiteSpace(username) || (b.User.UserName != null && b.User.UserName.Contains(username))))
                .Include(b => b.User)
                .Include(b => b.Tour)
                .Include(b => b.TourAcceptanceReports);

            return await query
                .Select(b => new ReportResponse
                {
                    Username = b.User.UserName ?? "Unknown",
                    BookingId = b.BookingId,
                    TourTitle = b.Tour.Title ?? "Unknown Tour",
                    Contract = b.Contract ?? "N/A",
                    TotalPrice = b.TotalPrice ?? 0,
                    TotalExtraCost = b.TourAcceptanceReports
                        .Where(r => r.IsActive)
                        .Sum(r => r.TotalExtraCost) ?? 0,
                    Total = (b.TotalPrice ?? 0) + (b.TourAcceptanceReports
                        .Where(r => r.IsActive)
                        .Sum(r => r.TotalExtraCost) ?? 0)
                })
                .ToListAsync();
        }
    }
}