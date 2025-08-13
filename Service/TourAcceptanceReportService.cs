using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Service
{
    public class TourAcceptanceReportService : ITourAcceptanceReportService
    {
        private readonly MyDBContext _context;
        private readonly INotificationService _notificationService;

        public TourAcceptanceReportService(MyDBContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<List<TourAcceptanceReportResponse>> GetReportsByGuideUserIdAsync(int userId)
        {
            var guide = await _context.TourGuides
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            if (guide == null) return new List<TourAcceptanceReportResponse>();

            var reports = await _context.TourAcceptanceReports
                .Where(r => r.TourGuideId == guide.TourGuideId && r.IsActive)
                .Include(r => r.GuideNotes.Where(gn => gn.IsActive))
                    .ThenInclude(gn => gn.GuideNoteMedia.Where(gnm => gnm.IsActive))
                .Include(r => r.Booking)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();

            return reports.Select(r => new TourAcceptanceReportResponse
            {
                ReportId = r.ReportId,
                BookingId = r.BookingId,
                TourGuideId = r.TourGuideId,
                TourGuideName = guide.User?.UserName ?? "Unknown Guide",
                ReportDate = r.ReportDate,
                Summary = r.Notes, // Sử dụng Notes field làm Summary
                TotalExtraCost = r.TotalExtraCost,
                Notes = r.Notes,
                AttachmentUrl = r.AttachmentUrl,
                IsActive = r.IsActive,
                GuideNotes = r.GuideNotes.Select(gn => new GuideNoteResponse
                {
                    NoteId = gn.NoteId,
                    BookingId = gn.BookingId,
                    AssignmentId = gn.AssignmentId,
                    Title = gn.Title,
                    Content = gn.Content,
                    CreatedAt = gn.CreatedAt,
                    MediaUrls = gn.GuideNoteMedia.Select(gnm => gnm.MediaUrl).ToList()
                }).ToList()
            }).ToList();
        }

        public async Task<TourAcceptanceReportResponse> GetReportByIdAsync(int reportId, int userId)
        {
            var guide = await _context.TourGuides
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            if (guide == null) throw new Exception("Guide not found");

            var report = await _context.TourAcceptanceReports
                .Include(r => r.GuideNotes.Where(gn => gn.IsActive))
                    .ThenInclude(gn => gn.GuideNoteMedia.Where(gnm => gnm.IsActive))
                .Include(r => r.Booking)
                .FirstOrDefaultAsync(r => r.ReportId == reportId && r.TourGuideId == guide.TourGuideId && r.IsActive);

            if (report == null) throw new Exception("Report not found");

            return new TourAcceptanceReportResponse
            {
                ReportId = report.ReportId,
                BookingId = report.BookingId,
                TourGuideId = report.TourGuideId,
                TourGuideName = guide.User?.UserName ?? "Unknown Guide",
                ReportDate = report.ReportDate,
                Summary = report.Notes,
                TotalExtraCost = report.TotalExtraCost,
                Notes = report.Notes,
                AttachmentUrl = report.AttachmentUrl,
                IsActive = report.IsActive,
                GuideNotes = report.GuideNotes.Select(gn => new GuideNoteResponse
                {
                    NoteId = gn.NoteId,
                    BookingId = gn.BookingId,
                    AssignmentId = gn.AssignmentId,
                    Title = gn.Title,
                    Content = gn.Content,
                    CreatedAt = gn.CreatedAt,
                    MediaUrls = gn.GuideNoteMedia.Select(gnm => gnm.MediaUrl).ToList()
                }).ToList()
            };
        }

        public async Task<TourAcceptanceReportResponse> CreateReportAsync(int userId, CreateTourAcceptanceReportRequest request)
        {
            var guide = await _context.TourGuides
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            if (guide == null) throw new Exception("Guide not found");

            // Kiểm tra assignment có thuộc guide không
            var assignment = await _context.TourGuideAssignments
                .FirstOrDefaultAsync(a => a.Id == request.AssignmentId && a.TourGuideId == guide.TourGuideId && a.IsActive);
            if (assignment == null) throw new Exception("Assignment not found");

            // Kiểm tra booking có tồn tại không
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == request.BookingId && b.IsActive);
            if (booking == null) throw new Exception("Booking not found");

            // Kiểm tra xem đã có report cho booking này chưa
            var existingReport = await _context.TourAcceptanceReports
                .FirstOrDefaultAsync(r => r.BookingId == request.BookingId && r.TourGuideId == guide.TourGuideId && r.IsActive);
            if (existingReport != null) throw new Exception("Report already exists for this booking");

            var report = new TourAcceptanceReport
            {
                BookingId = request.BookingId,
                TourGuideId = guide.TourGuideId,
                ReportDate = DateTime.UtcNow,
                TotalExtraCost = request.TotalExtraCost ?? 0,
                Notes = request.Notes,
                AttachmentUrl = request.AttachmentUrl,
                IsActive = true
            };

            _context.TourAcceptanceReports.Add(report);
            await _context.SaveChangesAsync();

            // Tạo notification cho user liên quan đến booking
            await _notificationService.CreateTourAcceptanceReportNotificationAsync(booking.UserId, report.ReportId);

            return await GetReportByIdAsync(report.ReportId, userId);
        }

        public async Task<TourAcceptanceReportResponse> UpdateReportAsync(int userId, int reportId, UpdateTourAcceptanceReportRequest request)
        {
            var guide = await _context.TourGuides
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            if (guide == null) throw new Exception("Guide not found");

            var report = await _context.TourAcceptanceReports
                .FirstOrDefaultAsync(r => r.ReportId == reportId && r.TourGuideId == guide.TourGuideId && r.IsActive);
            if (report == null) throw new Exception("Report not found");

            report.TotalExtraCost = request.TotalExtraCost ?? report.TotalExtraCost;
            report.Notes = request.Notes ?? report.Notes;
            report.AttachmentUrl = request.AttachmentUrl ?? report.AttachmentUrl;

            await _context.SaveChangesAsync();

            return await GetReportByIdAsync(reportId, userId);
        }

        public async Task<bool> DeleteReportAsync(int userId, int reportId)
        {
            var guide = await _context.TourGuides
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            if (guide == null) throw new Exception("Guide not found");

            var report = await _context.TourAcceptanceReports
                .FirstOrDefaultAsync(r => r.ReportId == reportId && r.TourGuideId == guide.TourGuideId && r.IsActive);
            if (report == null) throw new Exception("Report not found");

            report.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<TourAcceptanceReportResponse>> GetReportsByBookingIdAsync(int bookingId)
        {
            var reports = await _context.TourAcceptanceReports
                .Where(r => r.BookingId == bookingId && r.IsActive)
                .Include(r => r.TourGuide)
                    .ThenInclude(tg => tg.User)
                .Include(r => r.GuideNotes.Where(gn => gn.IsActive))
                    .ThenInclude(gn => gn.GuideNoteMedia.Where(gnm => gnm.IsActive))
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();

            return reports.Select(r => new TourAcceptanceReportResponse
            {
                ReportId = r.ReportId,
                BookingId = r.BookingId,
                TourGuideId = r.TourGuideId,
                TourGuideName = r.TourGuide.User?.UserName ?? "Unknown Guide",
                ReportDate = r.ReportDate,
                Summary = r.Notes,
                TotalExtraCost = r.TotalExtraCost,
                Notes = r.Notes,
                AttachmentUrl = r.AttachmentUrl,
                IsActive = r.IsActive,
                GuideNotes = r.GuideNotes.Select(gn => new GuideNoteResponse
                {
                    NoteId = gn.NoteId,
                    BookingId = gn.BookingId,
                    AssignmentId = gn.AssignmentId,
                    Title = gn.Title,
                    Content = gn.Content,
                    CreatedAt = gn.CreatedAt,
                    MediaUrls = gn.GuideNoteMedia.Select(gnm => gnm.MediaUrl).ToList()
                }).ToList()
            }).ToList();
        }
    }
} 