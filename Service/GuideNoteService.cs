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
    public class GuideNoteService : IGuideNoteService
    {
        private readonly MyDBContext _context;
        private readonly INotificationService _notificationService;

        public GuideNoteService(MyDBContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<List<GuideNoteResponse>> GetNotesByGuideUserIdAsync(int userId)
        {
            // Lấy TourGuideId từ UserId
            var guide = await _context.TourGuides
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            if (guide == null) return new List<GuideNoteResponse>();
            // Lấy tất cả assignment của guide
            var assignmentIds = await _context.TourGuideAssignments
                .Where(a => a.TourGuideId == guide.TourGuideId && a.IsActive)
                .Select(a => a.Id).ToListAsync();
            // Lấy note theo assignment
            var notes = await _context.GuideNotes
                .Where(n => assignmentIds.Contains(n.AssignmentId) && n.IsActive)
                .Include(n => n.GuideNoteMedia)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return notes.Select(n => new GuideNoteResponse
            {
                NoteId = n.NoteId,
                AssignmentId = n.AssignmentId,
                ReportId = n.ReportId,
                Title = n.Title,
                Content = n.Content,
                ExtraCost = n.ExtraCost,
                CreatedAt = n.CreatedAt,
                MediaUrls = n.GuideNoteMedia.Where(m => m.IsActive).Select(m => m.MediaUrl).ToList()
            }).ToList();
        }

        public async Task CreateNoteAsync(int userId, CreateGuideNoteRequest request)
        {
            var guide = await _context.TourGuides
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            if (guide == null) throw new Exception("Guide not found");
            
            // Kiểm tra assignment có thuộc guide không
            var assignment = await _context.TourGuideAssignments.FirstOrDefaultAsync(a => a.Id == request.AssignmentId && a.TourGuideId == guide.TourGuideId && a.IsActive);
            if (assignment == null) throw new Exception("Assignment not found");

            // Tìm hoặc tạo TourAcceptanceReport cho booking này
            /*var report = await _context.TourAcceptanceReports
                .FirstOrDefaultAsync(r => r.BookingId == assignment.BookingId && r.TourGuideId == guide.TourGuideId && r.IsActive);*/

            /*if (report == null)
            {
                // Tạo report mới nếu chưa có
                report = new TourAcceptanceReport
                {
                    BookingId = assignment.BookingId,
                    TourGuideId = guide.TourGuideId,
                    ReportDate = DateTime.UtcNow,
                    TotalExtraCost = 0,
                    Notes = "Auto-generated report",
                    IsActive = true
                };
                _context.TourAcceptanceReports.Add(report);
                await _context.SaveChangesAsync();
            }*/

            var note = new GuideNote
            {
                AssignmentId = request.AssignmentId,
                //ReportId = report.ReportId,
                Title = request.Title,
                Content = request.Content,
                ExtraCost = request.ExtraCost ?? 0,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _context.GuideNotes.Add(note);
            await _context.SaveChangesAsync();

            // Cập nhật tổng extra cost trong report
            var totalExtraCost = await _context.GuideNotes
                //.Where(gn => gn.ReportId == report.ReportId && gn.IsActive)
                .SumAsync(gn => gn.ExtraCost ?? 0);
            //report.TotalExtraCost = totalExtraCost;
            await _context.SaveChangesAsync();

            // Tạo notification cho user liên quan đến booking
            var booking = await _context.TourGuideAssignments
                .Where(a => a.Id == request.AssignmentId)
                //.Select(a => a.Booking)
                .FirstOrDefaultAsync();
            
            if (booking != null)
            {
                //await _notificationService.CreateGuideNoteNotificationAsync(booking.UserId, note.NoteId);
            }

            // Thêm media nếu có
            if (request.MediaUrls != null && request.MediaUrls.Count > 0)
            {
                foreach (var url in request.MediaUrls)
                {
                    var media = new GuideNoteMedia
                    {
                        NoteId = note.NoteId,
                        MediaUrl = url,
                        UploadedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    _context.GuideNoteMedia.Add(media);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateNoteAsync(int userId, int noteId, UpdateGuideNoteRequest request)
        {
            var guide = await _context.TourGuides
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            if (guide == null) throw new Exception("Guide not found");
            // Lấy note và kiểm tra quyền
            var note = await _context.GuideNotes.Include(n => n.Assignment).FirstOrDefaultAsync(n => n.NoteId == noteId && n.IsActive);
            if (note == null) throw new Exception("Note not found");
            if (note.Assignment.TourGuideId != guide.TourGuideId) throw new Exception("Not your note");
            note.Title = request.Title;
            note.Content = request.Content;
            // Xoá media cũ
            var oldMedia = await _context.GuideNoteMedia.Where(m => m.NoteId == noteId && m.IsActive).ToListAsync();
            foreach (var m in oldMedia) { m.IsActive = false; }
            // Thêm media mới
            if (request.MediaUrls != null && request.MediaUrls.Count > 0)
            {
                foreach (var url in request.MediaUrls)
                {
                    var media = new GuideNoteMedia
                    {
                        NoteId = note.NoteId,
                        MediaUrl = url,
                        UploadedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    _context.GuideNoteMedia.Add(media);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNoteAsync(int userId, int noteId)
        {
            var guide = await _context.TourGuides
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            if (guide == null) throw new Exception("Guide not found");
            var note = await _context.GuideNotes.Include(n => n.Assignment).FirstOrDefaultAsync(n => n.NoteId == noteId && n.IsActive);
            if (note == null) throw new Exception("Note not found");
            if (note.Assignment.TourGuideId != guide.TourGuideId) throw new Exception("Not your note");
            note.IsActive = false;
            // Xoá media liên quan
            var medias = await _context.GuideNoteMedia.Where(m => m.NoteId == noteId && m.IsActive).ToListAsync();
            foreach (var m in medias) { m.IsActive = false; }
            await _context.SaveChangesAsync();
        }
    }
} 