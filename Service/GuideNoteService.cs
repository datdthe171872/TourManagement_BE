using System;
using System.Collections.Generic;
using System.IO;
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
            
            // Lấy tất cả assignment của guide với thông tin tour và departure date
            var assignments = await _context.TourGuideAssignments
                .Include(a => a.DepartureDate)
                .Include(a => a.DepartureDate.Tour)
                .Where(a => a.TourGuideId == guide.TourGuideId && a.IsActive)
                .ToListAsync();
            
            var assignmentIds = assignments.Select(a => a.Id).ToList();
            
            // Lấy note theo assignment với thông tin đầy đủ
            var notes = await _context.GuideNotes
                .Where(n => assignmentIds.Contains(n.AssignmentId) && n.IsActive)
                .Include(n => n.GuideNoteMedia)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            
            return notes.Select(n => 
            {
                var assignment = assignments.FirstOrDefault(a => a.Id == n.AssignmentId);
                return new GuideNoteResponse
                {
                    NoteId = n.NoteId,
                    AssignmentId = n.AssignmentId,
                    ReportId = n.ReportId,
                    Title = n.Title,
                    Content = n.Content,
                    ExtraCost = n.ExtraCost,
                    CreatedAt = n.CreatedAt,
                    MediaUrls = n.GuideNoteMedia.Where(m => m.IsActive).Select(m => m.MediaUrl).ToList(),
                    TourGuideName = guide.User?.UserName,
                    TourTitle = assignment?.DepartureDate?.Tour?.Title,
                    DepartureDate = assignment?.DepartureDate?.DepartureDate1
                };
            }).ToList();
        }

        //public async Task CreateNoteAsync(int userId, CreateGuideNoteRequest request)
        //{
        //    var guide = await _context.TourGuides
        //        .Include(g => g.User)
        //        .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
        //    if (guide == null) throw new Exception("Guide not found");
            
        //    // Kiểm tra assignment có thuộc guide không
        //    var assignment = await _context.TourGuideAssignments.FirstOrDefaultAsync(a => a.Id == request.AssignmentId && a.TourGuideId == guide.TourGuideId && a.IsActive);
        //    if (assignment == null) throw new Exception("Assignment not found");
            
        //    // Tìm hoặc tạo TourAcceptanceReport cho booking này
        //    var report = await _context.TourAcceptanceReports
        //        .FirstOrDefaultAsync(r => r.BookingId == assignment.BookingId && r.TourGuideId == guide.TourGuideId && r.IsActive);
            
        //    if (report == null)
        //    {
        //        // Tạo report mới nếu chưa có
        //        report = new TourAcceptanceReport
        //        {
        //            BookingId = assignment.BookingId,
        //            TourGuideId = guide.TourGuideId,
        //            ReportDate = DateTime.UtcNow,
        //            TotalExtraCost = 0,
        //            Notes = "Auto-generated report",
        //            IsActive = true
        //        };
        //        _context.TourAcceptanceReports.Add(report);
        //        await _context.SaveChangesAsync();
        //    }
            
        //    var note = new GuideNote
        //    {
        //        AssignmentId = request.AssignmentId,
        //        ReportId = report.ReportId,
        //        Title = request.Title,
        //        Content = request.Content,
        //        ExtraCost = request.ExtraCost ?? 0,
        //        CreatedAt = DateTime.UtcNow,
        //        IsActive = true
        //    };
        //    _context.GuideNotes.Add(note);
        //    await _context.SaveChangesAsync();

        //    // Cập nhật tổng extra cost trong report
        //    var totalExtraCost = await _context.GuideNotes
        //        .Where(gn => gn.ReportId == report.ReportId && gn.IsActive)
        //        .SumAsync(gn => gn.ExtraCost ?? 0);
        //    report.TotalExtraCost = totalExtraCost;
        //    await _context.SaveChangesAsync();

        //    // Tạo notification cho user liên quan đến booking
        //    //var booking = await _context.TourGuideAssignments
        //    //    .Where(a => a.Id == request.AssignmentId)
        //    //    .Select(a => a.Booking)
        //    //    .FirstOrDefaultAsync();
            
        //    //if (booking != null)
        //    //{
        //    //    await _notificationService.CreateGuideNoteNotificationAsync(booking.UserId, note.NoteId);
        //    //}

        //    // Thêm media nếu có
        //    if (request.MediaUrls != null && request.MediaUrls.Count > 0)
        //    {
        //        foreach (var url in request.MediaUrls)
        //        {
        //            var media = new GuideNoteMedia
        //            {
        //                NoteId = note.NoteId,
        //                MediaUrl = url,
        //                UploadedAt = DateTime.UtcNow,
        //                IsActive = true
        //            };
        //            _context.GuideNoteMedia.Add(media);
        //        }
        //        await _context.SaveChangesAsync();
        //    }
        //}

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
            note.ExtraCost = request.ExtraCost ?? note.ExtraCost; // Cập nhật ExtraCost nếu có
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

        public async Task CreateNoteByTourGuideAsync(int userId, CreateGuideNoteByTourGuideRequest request)
        {
            try
            {
                var guide = await _context.TourGuides
                    .Include(g => g.User)
                    .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
                if (guide == null) throw new Exception("Guide not found");
                
                // Kiểm tra booking có tồn tại không
                var booking = await _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.DepartureDate)
                    .FirstOrDefaultAsync(b => b.BookingId == request.BookingId && b.IsActive);
                if (booking == null) throw new Exception("Booking not found");
                
                // Kiểm tra guide có được assign cho departure date này không
                var assignment = await _context.TourGuideAssignments
                    .FirstOrDefaultAsync(a => a.TourGuideId == guide.TourGuideId && 
                                            a.DepartureDateId == booking.DepartureDateId && 
                                            a.IsActive);
                if (assignment == null) throw new Exception("You are not assigned to this departure date");
                
                // Tìm hoặc tạo TourAcceptanceReport cho booking này
                var report = await _context.TourAcceptanceReports
                    .FirstOrDefaultAsync(r => r.BookingId == request.BookingId && 
                                            r.TourGuideId == guide.TourGuideId && 
                                            r.IsActive);
                
                if (report == null)
                {
                    // Tạo report mới nếu chưa có
                    report = new TourAcceptanceReport
                    {
                        BookingId = request.BookingId,
                        TourGuideId = guide.TourGuideId,
                        ReportDate = DateTime.UtcNow,
                        TotalExtraCost = 0,
                        Notes = "Auto-generated report",
                        IsActive = true
                    };
                    _context.TourAcceptanceReports.Add(report);
                    await _context.SaveChangesAsync();
                }
                
                // Tạo note mới
                var note = new GuideNote
                {
                    AssignmentId = assignment.Id,
                    ReportId = report.ReportId,
                    BookingId = request.BookingId,
                    DepartureDateId = booking.DepartureDateId,
                    Title = request.Title,
                    Content = request.Content,
                    ExtraCost = request.ExtraCost ?? 0, // TourGuide có thể set extraCost
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                
                _context.GuideNotes.Add(note);
                await _context.SaveChangesAsync();

                // Thêm media nếu có
                if (request.AttachmentUrls != null && request.AttachmentUrls.Count > 0)
                {
                    foreach (var url in request.AttachmentUrls)
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

                // Cập nhật tổng extra cost trong report
                var totalExtraCost = await _context.GuideNotes
                    .Where(gn => gn.ReportId == report.ReportId && gn.IsActive)
                    .SumAsync(gn => gn.ExtraCost ?? 0);
                report.TotalExtraCost = totalExtraCost;
                await _context.SaveChangesAsync();

                // Tạo notification cho customer
                await _notificationService.CreateNotificationAsync(
                    booking.UserId,
                    "New Guide Note",
                    $"Tour guide {guide.User.UserName} has added a note to your booking #{request.BookingId}",
                    "GuideNote",
                    note.NoteId.ToString()
                );
            }
            catch (Exception ex)
            {
                // Log the detailed error for debugging
                Console.WriteLine($"Error in CreateNoteByTourGuideAsync: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                throw new Exception($"Failed to create note: {ex.Message}");
            }
        }

        public async Task CreateNoteWithAttachmentAsync(int userId, CreateGuideNoteWithAttachmentRequest request)
        {
            try
            {
                var guide = await _context.TourGuides
                    .Include(g => g.User)
                    .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
                if (guide == null) throw new Exception("Guide not found");
                
                // Kiểm tra booking có tồn tại không
                var booking = await _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.DepartureDate)
                    .FirstOrDefaultAsync(b => b.BookingId == request.BookingId && b.IsActive);
                if (booking == null) throw new Exception("Booking not found");
                
                // Kiểm tra guide có được assign cho departure date này không
                var assignment = await _context.TourGuideAssignments
                    .FirstOrDefaultAsync(a => a.TourGuideId == guide.TourGuideId && 
                                            a.DepartureDateId == booking.DepartureDateId && 
                                            a.IsActive);
                if (assignment == null) throw new Exception("You are not assigned to this departure date");
                
                // Tìm hoặc tạo TourAcceptanceReport cho booking này
                var report = await _context.TourAcceptanceReports
                    .FirstOrDefaultAsync(r => r.BookingId == request.BookingId && 
                                            r.TourGuideId == guide.TourGuideId && 
                                            r.IsActive);
                
                if (report == null)
                {
                    // Tạo report mới nếu chưa có
                    report = new TourAcceptanceReport
                    {
                        BookingId = request.BookingId,
                        TourGuideId = guide.TourGuideId,
                        ReportDate = DateTime.UtcNow,
                        TotalExtraCost = 0,
                        Notes = "Auto-generated report",
                        IsActive = true
                    };
                    _context.TourAcceptanceReports.Add(report);
                    await _context.SaveChangesAsync();
                }
                
                // Tạo note mới
                var note = new GuideNote
                {
                    AssignmentId = assignment.Id,
                    ReportId = report.ReportId,
                    BookingId = request.BookingId,
                    DepartureDateId = booking.DepartureDateId,
                    Title = request.Title,
                    Content = request.Content,
                    ExtraCost = request.ExtraCost ?? 0, // TourGuide có thể set extraCost
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                
                _context.GuideNotes.Add(note);
                await _context.SaveChangesAsync();

                // Upload và thêm media nếu có
                if (request.Attachments != null && request.Attachments.Count > 0)
                {
                    foreach (var file in request.Attachments)
                    {
                        if (file != null && file.Length > 0)
                        {
                            // Kiểm tra file type
                            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
                            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                            
                            if (!allowedExtensions.Contains(fileExtension))
                            {
                                throw new Exception($"Invalid file type: {fileExtension}. Allowed: jpg, jpeg, png, gif, pdf");
                            }

                            // Kiểm tra file size (max 10MB)
                            if (file.Length > 10 * 1024 * 1024)
                            {
                                throw new Exception($"File {file.FileName} is too large. Maximum 10MB allowed");
                            }

                            // Tạo unique filename
                            var fileName = $"{Guid.NewGuid()}{fileExtension}";
                            var uploadPath = Path.Combine("wwwroot", "uploads", "guidenotes");
                            
                            // Tạo thư mục nếu chưa tồn tại
                            if (!Directory.Exists(uploadPath))
                            {
                                Directory.CreateDirectory(uploadPath);
                            }

                            var filePath = Path.Combine(uploadPath, fileName);
                            
                            // Lưu file
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Tạo URL
                            var fileUrl = $"/uploads/guidenotes/{fileName}";
                            
                            // Thêm vào database
                            var media = new GuideNoteMedia
                            {
                                NoteId = note.NoteId,
                                MediaUrl = fileUrl,
                                UploadedAt = DateTime.UtcNow,
                                IsActive = true
                            };
                            _context.GuideNoteMedia.Add(media);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Cập nhật tổng extra cost trong report
                var totalExtraCost = await _context.GuideNotes
                    .Where(gn => gn.ReportId == report.ReportId && gn.IsActive)
                    .SumAsync(gn => gn.ExtraCost ?? 0);
                report.TotalExtraCost = totalExtraCost;
                await _context.SaveChangesAsync();

                // Tạo notification cho customer
                await _notificationService.CreateNotificationAsync(
                    booking.UserId,
                    "New Guide Note",
                    $"Tour guide {guide.User.UserName} has added a note to your booking #{request.BookingId}",
                    "GuideNote",
                    note.NoteId.ToString()
                );
            }
            catch (Exception ex)
            {
                // Log the detailed error for debugging
                Console.WriteLine($"Error in CreateNoteWithAttachmentAsync: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                throw new Exception($"Failed to create note with attachment: {ex.Message}");
            }
        }

        public async Task<List<TourGuideBookingResponse>> GetMyBookingsAsync(int userId)
        {
            var guide = await _context.TourGuides
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            if (guide == null) return new List<TourGuideBookingResponse>();

            // Lấy tất cả departure dates mà guide được assign
            var assignments = await _context.TourGuideAssignments
                .Include(a => a.DepartureDate)
                .ThenInclude(dd => dd.Tour)
                .Where(a => a.TourGuideId == guide.TourGuideId && a.IsActive)
                .ToListAsync();

            var bookings = new List<TourGuideBookingResponse>();

            foreach (var assignment in assignments)
            {
                // Lấy tất cả booking cho departure date này
                var departureBookings = await _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.DepartureDate)
                    .Include(b => b.Tour)
                    .Where(b => b.DepartureDateId == assignment.DepartureDateId && b.IsActive)
                    .ToListAsync();

                foreach (var booking in departureBookings)
                {
                    bookings.Add(new TourGuideBookingResponse
                    {
                        BookingId = booking.BookingId,
                        TourId = booking.TourId,
                        DepartureDateId = booking.DepartureDateId,
                        TourTitle = booking.Tour?.Title ?? "Unknown",
                        DepartureDate = booking.DepartureDate?.DepartureDate1 ?? DateTime.MinValue,
                        CustomerName = booking.User?.UserName ?? "Unknown",
                        CustomerEmail = booking.User?.Email ?? "",
                        CustomerPhone = booking.User?.PhoneNumber ?? "",
                        NumberOfAdults = booking.NumberOfAdults ?? 0,
                        NumberOfChildren = booking.NumberOfChildren ?? 0,
                        NumberOfInfants = booking.NumberOfInfants ?? 0,
                        NoteForTour = booking.NoteForTour,
                        TotalPrice = booking.TotalPrice ?? 0,
                        BookingStatus = booking.BookingStatus,
                        PaymentStatus = booking.PaymentStatus,
                        BookingDate = booking.BookingDate,
                        IsLeadGuide = assignment.IsLeadGuide ?? false,
                        AssignedDate = assignment.AssignedDate?.ToDateTime(TimeOnly.MinValue)
                    });
                }
            }

            return bookings.OrderByDescending(b => b.DepartureDate).ToList();
        }



        public async Task<List<GuideNoteResponse>> GetNotesByTourOperatorAsync(int userId)
        {
            // Lấy TourOperator từ UserId
            var tourOperator = await _context.TourOperators
                .Include(to => to.User)
                .FirstOrDefaultAsync(to => to.UserId == userId && to.IsActive);
            
            if (tourOperator == null)
            {
                Console.WriteLine($"Debug - TourOperator with UserId {userId} not found or not active");
                throw new Exception($"TourOperator with UserId {userId} not found");
            }
            
            Console.WriteLine($"Debug - Found TourOperator: {tourOperator.CompanyName}, TourOperatorId: {tourOperator.TourOperatorId}");

            // Lấy tất cả note của các TourGuide thuộc tour của TourOperator này
            var notes = await _context.GuideNotes
                .Include(n => n.Booking)
                .ThenInclude(b => b.Tour)
                .Include(n => n.GuideNoteMedia)
                .Include(n => n.Assignment)
                .ThenInclude(a => a.TourGuide)
                .ThenInclude(tg => tg.User)
                .Include(n => n.Booking)
                .ThenInclude(b => b.DepartureDate)
                .Where(n => n.IsActive && 
                           n.Booking.Tour.TourOperatorId == tourOperator.TourOperatorId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            Console.WriteLine($"Debug - Found {notes.Count} notes for TourOperator {tourOperator.TourOperatorId}");

            return notes.Select(n => new GuideNoteResponse
            {
                NoteId = n.NoteId,
                AssignmentId = n.AssignmentId,
                ReportId = n.ReportId,
                Title = n.Title,
                Content = n.Content,
                ExtraCost = n.ExtraCost,
                CreatedAt = n.CreatedAt,
                MediaUrls = n.GuideNoteMedia.Where(m => m.IsActive).Select(m => m.MediaUrl).ToList(),
                TourGuideName = n.Assignment.TourGuide.User?.UserName ?? "Unknown",
                TourTitle = n.Booking.Tour?.Title ?? "Unknown",
                DepartureDate = n.Booking.DepartureDate?.DepartureDate1 ?? DateTime.MinValue
            }).ToList();
        }
    }
} 