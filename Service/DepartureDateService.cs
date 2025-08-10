using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.DepartureDatesRequest;
using TourManagement_BE.Data.DTO.Response.DepartureDateResponse;
using BookingResponse = TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Helper.Constant;
using TourManagement_BE.Helper.Common;

namespace TourManagement_BE.Service;

public class DepartureDateService : IDepartureDateService
{
    private readonly MyDBContext _context;
    private readonly INotificationService _notificationService;
    private readonly EmailHelper _emailHelper;

    public DepartureDateService(MyDBContext context, INotificationService notificationService, EmailHelper emailHelper)
    {
        _context = context;
        _notificationService = notificationService;
        _emailHelper = emailHelper;
    }

    public async Task<DepartureDate?> CreateDepartureDatesAsync(CreateDepartureDateRequest request, int userId)
    {
        // Kiểm tra tour có tồn tại không và thuộc về Tour Operator đang đăng nhập
        var tour = await _context.Tours
            .Include(t => t.TourOperator)
            .FirstOrDefaultAsync(t => t.TourId == request.TourId && t.IsActive);
        
        if (tour == null)
            return null;

        // Kiểm tra quyền sở hữu tour
        if (tour.TourOperator.UserId != userId)
            return null;

        // Kiểm tra ngày bắt đầu không được trong quá khứ
        if (request.StartDate.Date <= DateTime.Now.Date)
            return null;

        // Kiểm tra khoảng cách với tất cả departure dates hiện có
        var existingDepartureDates = await _context.DepartureDates
            .Where(dd => dd.TourId == request.TourId && dd.IsActive)
            .ToListAsync();

        if (existingDepartureDates.Any())
        {
            foreach (var existingDd in existingDepartureDates)
            {
                var daysDifference = Math.Abs((request.StartDate.Date - existingDd.DepartureDate1.Date).Days);
                
                // Nếu khoảng cách ít hơn 1 ngày, trả về lỗi
                if (daysDifference < 1)
                {
                    return null; // Sẽ được xử lý trong controller để trả về thông báo lỗi cụ thể
                }
            }
        }

        var departureDates = new List<DepartureDate>();
        var currentDate = request.StartDate;

        // Tạo chỉ 1 departure date
        var departureDate = new DepartureDate
        {
            TourId = request.TourId,
            DepartureDate1 = currentDate,
            IsActive = true
        };

        departureDates.Add(departureDate);

        await _context.DepartureDates.AddRangeAsync(departureDates);
        await _context.SaveChangesAsync();

        // Gửi notification và email
        await SendDepartureDateCreatedNotificationsAsync(departureDate, userId);

        // Trả về departure date vừa tạo với thông tin đầy đủ
        return departureDate;
    }

    public async Task<List<DepartureDateResponse>> GetAllDepartureDatesAsync()
    {
        var departureDates = await _context.DepartureDates
            .Include(dd => dd.Tour)
            .Include(dd => dd.Bookings.Where(b => b.IsActive))
            .Where(dd => dd.IsActive)
            .OrderBy(dd => dd.DepartureDate1)
            .Select(dd => new DepartureDateResponse
            {
                Id = dd.Id,
                TourId = dd.TourId,
                TourTitle = dd.Tour.Title,
                DepartureDate = dd.DepartureDate1,
                IsActive = dd.IsActive,
                IsCancelDate = dd.IsCancelDate,
                TotalBookings = dd.Bookings.Count,
                AvailableSlots = dd.Tour.MaxSlots - (dd.Tour.SlotsBooked ?? 0)
            })
            .ToListAsync();

        // Thêm thông tin TourGuide cho mỗi departureDate
        foreach (var departureDate in departureDates)
        {
            departureDate.TourGuides = await GetTourGuidesForDepartureDateAsync(departureDate.Id);
        }

        return departureDates;
    }

    public async Task<List<DepartureDateResponse>> GetDepartureDatesByTourIdAsync(int tourId)
    {
        var departureDates = await _context.DepartureDates
            .Include(dd => dd.Tour)
            .Include(dd => dd.Bookings.Where(b => b.IsActive))
            .Where(dd => dd.TourId == tourId && dd.IsActive)
            .OrderBy(dd => dd.DepartureDate1)
            .Select(dd => new DepartureDateResponse
            {
                Id = dd.Id,
                TourId = dd.TourId,
                TourTitle = dd.Tour.Title,
                DepartureDate = dd.DepartureDate1,
                IsActive = dd.IsActive,
                IsCancelDate = dd.IsCancelDate,
                TotalBookings = dd.Bookings.Count,
                AvailableSlots = dd.Tour.MaxSlots - (dd.Tour.SlotsBooked ?? 0)
            })
            .ToListAsync();

        // Thêm thông tin TourGuide cho mỗi departureDate
        foreach (var departureDate in departureDates)
        {
            departureDate.TourGuides = await GetTourGuidesForDepartureDateAsync(departureDate.Id);
        }

        return departureDates;
    }

    public async Task<List<DepartureDateWithBookingResponse>> GetDepartureDatesWithBookingsByTourOperatorAsync(int userId)
    {
        // Bước 1: Lấy TourOperatorId từ UserId
        var tourOperator = await _context.TourOperators
            .FirstOrDefaultAsync(to => to.UserId == userId && to.IsActive);

        if (tourOperator == null)
            return new List<DepartureDateWithBookingResponse>();

        // Bước 2: Lấy tất cả TourIds của TourOperator này
        var tourIds = await _context.Tours
            .Where(t => t.TourOperatorId == tourOperator.TourOperatorId && t.IsActive)
            .Select(t => t.TourId)
            .ToListAsync();

        if (!tourIds.Any())
            return new List<DepartureDateWithBookingResponse>();

        // Bước 3: Lấy tất cả DepartureDates với Booking của các Tour này
        var departureDatesWithBookings = await _context.DepartureDates
            .Include(dd => dd.Tour)
            .Include(dd => dd.Bookings.Where(b => b.IsActive))
                .ThenInclude(b => b.User)
            .Where(dd => tourIds.Contains(dd.TourId) && dd.IsActive)
            .OrderBy(dd => dd.DepartureDate1)
            .Select(dd => new DepartureDateWithBookingResponse
            {
                Id = dd.Id,
                TourId = dd.TourId,
                TourTitle = dd.Tour.Title,
                DepartureDate = dd.DepartureDate1,
                IsActive = dd.IsActive,
                IsCancelDate = dd.IsCancelDate,
                TotalBookings = dd.Bookings.Count,
                AvailableSlots = dd.Tour.MaxSlots - (dd.Tour.SlotsBooked ?? 0),
                Bookings = dd.Bookings.Select(b => new BookingInfo
                {
                    BookingId = b.BookingId,
                    UserId = b.UserId,
                    UserName = b.User.UserName ?? "Unknown",
                    UserEmail = b.User.Email,
                    BookingDate = b.BookingDate ?? DateTime.UtcNow,
                    NumberOfAdults = b.NumberOfAdults,
                    NumberOfChildren = b.NumberOfChildren,
                    NumberOfInfants = b.NumberOfInfants,
                    TotalPrice = b.TotalPrice,
                    BookingStatus = b.BookingStatus,
                    PaymentStatus = b.PaymentStatus
                }).ToList()
            })
            .ToListAsync();

        // Thêm thông tin TourGuide cho mỗi departureDate
        foreach (var departureDate in departureDatesWithBookings)
        {
            departureDate.TourGuides = await GetTourGuidesForDepartureDateAsync(departureDate.Id);
        }

        return departureDatesWithBookings;
    }

    public async Task<List<DepartureDateResponse>> GetDepartureDatesByTourOperatorAsync(int userId)
    {
        // Bước 1: Lấy TourOperatorId từ UserId
        var tourOperator = await _context.TourOperators
            .FirstOrDefaultAsync(to => to.UserId == userId && to.IsActive);

        if (tourOperator == null)
            return new List<DepartureDateResponse>();

        // Bước 2: Lấy tất cả TourIds của TourOperator này
        var tourIds = await _context.Tours
            .Where(t => t.TourOperatorId == tourOperator.TourOperatorId && t.IsActive)
            .Select(t => t.TourId)
            .ToListAsync();

        if (!tourIds.Any())
            return new List<DepartureDateResponse>();

        // Bước 3: Lấy tất cả DepartureDates của các Tour này
        var departureDates = await _context.DepartureDates
            .Include(dd => dd.Tour)
            .Include(dd => dd.Bookings.Where(b => b.IsActive))
            .Where(dd => tourIds.Contains(dd.TourId) && dd.IsActive)
            .OrderBy(dd => dd.DepartureDate1)
            .Select(dd => new DepartureDateResponse
            {
                Id = dd.Id,
                TourId = dd.TourId,
                TourTitle = dd.Tour.Title,
                DepartureDate = dd.DepartureDate1,
                IsActive = dd.IsActive,
                IsCancelDate = dd.IsCancelDate,
                TotalBookings = dd.Bookings.Count,
                AvailableSlots = dd.Tour.MaxSlots - (dd.Tour.SlotsBooked ?? 0)
            })
            .ToListAsync();

        // Thêm thông tin TourGuide cho mỗi departureDate
        foreach (var departureDate in departureDates)
        {
            departureDate.TourGuides = await GetTourGuidesForDepartureDateAsync(departureDate.Id);
        }

        return departureDates;
    }

    public async Task<DepartureDateBookingsWrapperResponse?> GetBookingsByDepartureDateIdAsync(int departureDateId, int userId)
    {
        // Kiểm tra xem user có phải là TourOperator không
        var tourOperator = await _context.TourOperators
            .FirstOrDefaultAsync(to => to.UserId == userId && to.IsActive);
        
        // Kiểm tra xem user có phải là TourGuide không
        var tourGuide = await _context.TourGuides
            .FirstOrDefaultAsync(tg => tg.UserId == userId && tg.IsActive);
        
        if (tourOperator == null && tourGuide == null)
            return null;
        
        // Query cơ bản cho DepartureDate
        var departureDateQuery = _context.DepartureDates
            .Include(dd => dd.Tour)
            .Include(dd => dd.Bookings.Where(b => b.IsActive))
                .ThenInclude(b => b.User)
            .Where(dd => dd.Id == departureDateId && dd.IsActive);
        
        // Nếu là TourOperator, kiểm tra quyền sở hữu tour
        if (tourOperator != null)
        {
            departureDateQuery = departureDateQuery.Where(dd => dd.Tour.TourOperatorId == tourOperator.TourOperatorId);
        }
        // Nếu là TourGuide, kiểm tra xem có được assign cho departureDate này không
        else if (tourGuide != null)
        {
            departureDateQuery = departureDateQuery.Where(dd => 
                _context.TourGuideAssignments.Any(tga => 
                    tga.DepartureDateId == dd.Id && 
                    tga.TourGuideId == tourGuide.TourGuideId && 
                    tga.IsActive));
        }
        
        var departureDate = await departureDateQuery.FirstOrDefaultAsync();
        if (departureDate == null)
            return null;
        var bookingDetails = new List<DepartureDateBookingDetailResponse>();
        foreach (var booking in departureDate.Bookings)
        {
            var bookingDetail = new DepartureDateBookingDetailResponse
            {
                BookingId = booking.BookingId,
                Tour = new TourDetailInfo
                {
                    Title = departureDate.Tour.Title,
                    MaxSlots = departureDate.Tour.MaxSlots,
                    Transportation = departureDate.Tour.Transportation,
                    StartPoint = departureDate.Tour.StartPoint,
                    DepartureDate = departureDate.DepartureDate1,
                    DurationInDays = departureDate.Tour.DurationInDays
                },
                Booking = new BookingDetailInfo
                {
                    BookingDate = booking.BookingDate ?? DateTime.UtcNow,
                    Contract = booking.Contract,
                    NoteForTour = booking.NoteForTour
                },
                Guest = new GuestDetailInfo
                {
                    NumberOfAdults = booking.NumberOfAdults ?? 0,
                    NumberOfChildren = booking.NumberOfChildren ?? 0,
                    NumberOfInfants = booking.NumberOfInfants ?? 0
                },
                BillingInfo = new BillingDetailInfo
                {
                    Username = booking.User.UserName,
                    Email = booking.User.Email,
                    Phone = booking.User.PhoneNumber,
                    Address = booking.User.Address
                },
                PaymentInfo = new PaymentDetailInfo
                {
                    TotalPrice = booking.TotalPrice,
                    PaymentStatus = booking.PaymentStatus,
                    BookingStatus = booking.BookingStatus
                },
                GuideNotes = new List<GuideNoteDetailInfo>()
            };
            bookingDetails.Add(bookingDetail);
        }
        return new DepartureDateBookingsWrapperResponse
        {
            DepartureDateId = departureDate.Id,
            TourTitle = departureDate.Tour.Title,
            DepartureDate = departureDate.DepartureDate1,
            Bookings = bookingDetails
        };
    }

    // Helper method để lấy thông tin TourGuide cho một departureDate
    private async Task<List<TourGuideInfo>> GetTourGuidesForDepartureDateAsync(int departureDateId)
    {
        var tourGuides = await _context.TourGuideAssignments
            .Include(tga => tga.TourGuide)
            .ThenInclude(tg => tg.User)
            .Where(tga => tga.DepartureDateId == departureDateId && tga.IsActive)
            .Select(tga => new TourGuideInfo
            {
                TourGuideId = tga.TourGuideId,
                UserId = tga.TourGuide.UserId,
                UserName = tga.TourGuide.User != null ? tga.TourGuide.User.UserName : null,
                Email = tga.TourGuide.User != null ? tga.TourGuide.User.Email : null,
                PhoneNumber = tga.TourGuide.User != null ? tga.TourGuide.User.PhoneNumber : null,
                IsActive = tga.TourGuide.IsActive,
                AssignmentId = tga.Id,
                AssignedDate = tga.AssignedDate,
                IsLeadGuide = tga.IsLeadGuide
            })
            .ToListAsync();

        return tourGuides;
    }

    public async Task<bool> CancelDepartureDateAsync(int departureDateId, int userId)
    {
        // Bước 1: Kiểm tra TourOperator có tồn tại không
        var tourOperator = await _context.TourOperators
            .FirstOrDefaultAsync(to => to.UserId == userId && to.IsActive);

        if (tourOperator == null)
            return false;

        // Bước 2: Kiểm tra DepartureDate có tồn tại và thuộc về TourOperator này không
        var departureDate = await _context.DepartureDates
            .Include(dd => dd.Tour)
            .Include(dd => dd.Bookings.Where(b => b.IsActive))
            .FirstOrDefaultAsync(dd => dd.Id == departureDateId && 
                                     dd.Tour.TourOperatorId == tourOperator.TourOperatorId && 
                                     dd.IsActive);

        if (departureDate == null)
            return false;

        // Bước 3: Kiểm tra ngày khởi hành chưa diễn ra
        if (departureDate.DepartureDate1.Date <= DateTime.Now.Date)
            return false;

        // Bước 4: Cập nhật trạng thái DepartureDate thành cancelled
        departureDate.IsCancelDate = true;
        // Giữ nguyên IsActive như yêu cầu

        // Bước 5: Cập nhật trạng thái tất cả Booking trong DepartureDate này thành Cancelled
        foreach (var booking in departureDate.Bookings)
        {
            booking.BookingStatus = BookingStatus.Cancelled;
        }

        // Bước 6: Lưu thay đổi vào database
        await _context.SaveChangesAsync();

        // Gửi notification và email
        await SendDepartureDateCancelledNotificationsAsync(departureDate, userId);

        return true;
    }

    public async Task<List<DepartureDateResponse>> GetCancelledDepartureDatesByTourOperatorAsync(int userId)
    {
        // Bước 1: Lấy TourOperatorId từ UserId
        var tourOperator = await _context.TourOperators
            .FirstOrDefaultAsync(to => to.UserId == userId && to.IsActive);

        if (tourOperator == null)
            return new List<DepartureDateResponse>();

        // Bước 2: Lấy tất cả TourIds của TourOperator này
        var tourIds = await _context.Tours
            .Where(t => t.TourOperatorId == tourOperator.TourOperatorId && t.IsActive)
            .Select(t => t.TourId)
            .ToListAsync();

        if (!tourIds.Any())
            return new List<DepartureDateResponse>();

        // Bước 3: Lấy tất cả DepartureDates đã bị hủy của các Tour này
        var cancelledDepartureDates = await _context.DepartureDates
            .Include(dd => dd.Tour)
            .Include(dd => dd.Bookings.Where(b => b.IsActive))
            .Where(dd => tourIds.Contains(dd.TourId) && 
                        dd.IsCancelDate)
            .OrderBy(dd => dd.DepartureDate1)
            .Select(dd => new DepartureDateResponse
            {
                Id = dd.Id,
                TourId = dd.TourId,
                TourTitle = dd.Tour.Title,
                DepartureDate = dd.DepartureDate1,
                IsActive = dd.IsActive,
                IsCancelDate = dd.IsCancelDate,
                TotalBookings = dd.Bookings.Count,
                AvailableSlots = dd.Tour.MaxSlots - (dd.Tour.SlotsBooked ?? 0)
            })
            .ToListAsync();

        // Thêm thông tin TourGuide cho mỗi departureDate
        foreach (var departureDate in cancelledDepartureDates)
        {
            departureDate.TourGuides = await GetTourGuidesForDepartureDateAsync(departureDate.Id);
        }

        return cancelledDepartureDates;
    }

    public async Task<bool> ReactivateDepartureDateAsync(int departureDateId, int userId)
    {
        // Bước 1: Kiểm tra TourOperator có tồn tại không
        var tourOperator = await _context.TourOperators
            .FirstOrDefaultAsync(to => to.UserId == userId && to.IsActive);

        if (tourOperator == null)
            return false;

        // Bước 2: Kiểm tra DepartureDate có tồn tại và thuộc về TourOperator này không
        var departureDate = await _context.DepartureDates
            .Include(dd => dd.Tour)
            .Include(dd => dd.Bookings.Where(b => b.IsActive))
            .FirstOrDefaultAsync(dd => dd.Id == departureDateId && 
                                     dd.Tour.TourOperatorId == tourOperator.TourOperatorId && 
                                     dd.IsCancelDate);

        if (departureDate == null)
            return false;

        // Bước 3: Kiểm tra ngày khởi hành phải cách hiện tại ít nhất 5 ngày
        var daysUntilDeparture = (departureDate.DepartureDate1.Date - DateTime.Now.Date).Days;
        if (daysUntilDeparture < 5)
            return false;

        // Bước 4: Cập nhật trạng thái DepartureDate thành active
        departureDate.IsCancelDate = false;
        // Giữ nguyên IsActive như yêu cầu

        // Bước 5: Khôi phục trạng thái tất cả Booking trong DepartureDate này thành Pending
        foreach (var booking in departureDate.Bookings)
        {
            booking.BookingStatus = BookingStatus.Pending;
        }

        // Bước 6: Lưu thay đổi vào database
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<DepartureDateResponse>> GetDepartureDatesByTourGuideAsync(int userId)
    {
        // Bước 1: Lấy TourGuideId từ UserId
        var tourGuide = await _context.TourGuides
            .FirstOrDefaultAsync(tg => tg.UserId == userId && tg.IsActive);

        if (tourGuide == null)
            return new List<DepartureDateResponse>();

        // Bước 2: Lấy tất cả DepartureDateIds mà TourGuide này được assign
        var departureDateIds = await _context.TourGuideAssignments
            .Where(tga => tga.TourGuideId == tourGuide.TourGuideId && tga.IsActive)
            .Select(tga => tga.DepartureDateId)
            .ToListAsync();

        if (!departureDateIds.Any())
            return new List<DepartureDateResponse>();

        // Bước 3: Lấy tất cả DepartureDates mà TourGuide được assign
        var departureDates = await _context.DepartureDates
            .Include(dd => dd.Tour)
            .Include(dd => dd.Bookings.Where(b => b.IsActive))
            .Where(dd => departureDateIds.Contains(dd.Id) && dd.IsActive)
            .OrderBy(dd => dd.DepartureDate1)
            .Select(dd => new DepartureDateResponse
            {
                Id = dd.Id,
                TourId = dd.TourId,
                TourTitle = dd.Tour.Title,
                DepartureDate = dd.DepartureDate1,
                IsActive = dd.IsActive,
                IsCancelDate = dd.IsCancelDate,
                TotalBookings = dd.Bookings.Count,
                AvailableSlots = dd.Tour.MaxSlots - (dd.Tour.SlotsBooked ?? 0)
            })
            .ToListAsync();

        // Thêm thông tin TourGuide cho mỗi departureDate
        foreach (var departureDate in departureDates)
        {
            departureDate.TourGuides = await GetTourGuidesForDepartureDateAsync(departureDate.Id);
        }

        return departureDates;
    }

    public async Task<DepartureDate?> UpdateDepartureDateAsync(UpdateDepartureDateRequest request, int userId)
    {
        // Kiểm tra departure date có tồn tại không
        var departureDate = await _context.DepartureDates
            .Include(dd => dd.Tour)
                .ThenInclude(t => t.TourOperator)
            .FirstOrDefaultAsync(dd => dd.Id == request.Id && dd.IsActive);
        
        if (departureDate == null)
            return null;

        // Kiểm tra quyền sở hữu - departure date phải thuộc về tour của tour operator hiện tại
        if (departureDate.Tour.TourOperator.UserId != userId)
            return null;

        // Kiểm tra ngày khởi hành mới không được trong quá khứ
        if (request.DepartureDate1.Date <= DateTime.Now.Date)
            return null;

        // Kiểm tra có booking nào đã confirmed cho departure date này không
        var hasConfirmedBookings = await _context.Bookings
            .AnyAsync(b => b.DepartureDateId == request.Id && 
                          b.IsActive && 
                          (b.BookingStatus == StatusConstants.Booking.Confirmed || 
                           b.BookingStatus == StatusConstants.Booking.Completed));

        if (hasConfirmedBookings)
            return null; // Không thể update nếu đã có booking confirmed

        // Kiểm tra khoảng cách với tất cả departure dates khác (trừ departure date hiện tại)
        var otherDepartureDates = await _context.DepartureDates
            .Where(dd => dd.TourId == departureDate.TourId && 
                        dd.IsActive && 
                        dd.Id != request.Id)
            .ToListAsync();

        if (otherDepartureDates.Any())
        {
            foreach (var otherDd in otherDepartureDates)
            {
                var daysDifference = Math.Abs((request.DepartureDate1.Date - otherDd.DepartureDate1.Date).Days);
                if (daysDifference < 1)
                    return null; // Khoảng cách không đủ (ít nhất 1 ngày)
            }
        }

        // Cập nhật departure date
        departureDate.DepartureDate1 = request.DepartureDate1;
        
        await _context.SaveChangesAsync();

        // Gửi notification và email
        await SendDepartureDateUpdatedNotificationsAsync(departureDate, userId);

        return departureDate;
    }

    // Notification methods for departure date operations
    public async Task SendDepartureDateCreatedNotificationsAsync(DepartureDate departureDate, int tourOperatorId)
    {
        try
        {
            // Get tour information
            var tour = await _context.Tours
                .Include(t => t.TourOperator)
                .ThenInclude(to => to.User)
                .FirstOrDefaultAsync(t => t.TourId == departureDate.TourId);

            if (tour == null) return;

            // Send notification to Tour Operator
            await _notificationService.CreateNotificationAsync(
                tourOperatorId,
                "Ngày khởi hành mới đã được tạo",
                $"Ngày khởi hành {departureDate.DepartureDate1:dd/MM/yyyy} cho tour '{tour.Title}' đã được tạo thành công.",
                StatusConstants.NotificationType.DepartureDateCreated,
                departureDate.Id.ToString()
            );

            // Send email to Tour Operator
            var tourOperatorEmail = tour.TourOperator.User.Email;
            if (!string.IsNullOrEmpty(tourOperatorEmail))
            {
                var emailSubject = "Ngày khởi hành mới đã được tạo";
                var emailBody = $@"
                    <h3>Thông báo tạo ngày khởi hành mới</h3>
                    <p>Xin chào {tour.TourOperator.User.UserName},</p>
                    <p>Ngày khởi hành mới đã được tạo thành công:</p>
                    <ul>
                        <li><strong>Tour:</strong> {tour.Title}</li>
                        <li><strong>Ngày khởi hành:</strong> {departureDate.DepartureDate1:dd/MM/yyyy}</li>
                        <li><strong>Thời gian tạo:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</li>
                    </ul>
                    <p>Bạn có thể quản lý ngày khởi hành này trong hệ thống.</p>
                    <p>Trân trọng,<br>Hệ thống quản lý tour</p>";

                await _emailHelper.SendEmailAsync(tourOperatorEmail, emailSubject, emailBody);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't throw to avoid breaking the main operation
            Console.WriteLine($"Error sending departure date created notifications: {ex.Message}");
        }
    }

    public async Task SendDepartureDateUpdatedNotificationsAsync(DepartureDate departureDate, int tourOperatorId)
    {
        try
        {
            // Get tour information
            var tour = await _context.Tours
                .Include(t => t.TourOperator)
                .ThenInclude(to => to.User)
                .FirstOrDefaultAsync(t => t.TourId == departureDate.TourId);

            if (tour == null) return;

            // Get all users with bookings for this departure date
            var bookingUsers = await _context.Bookings
                .Include(b => b.User)
                .Where(b => b.DepartureDateId == departureDate.Id && b.IsActive)
                .Select(b => b.User)
                .Distinct()
                .ToListAsync();

            // Send notification to Tour Operator
            await _notificationService.CreateNotificationAsync(
                tourOperatorId,
                "Ngày khởi hành đã được cập nhật",
                $"Ngày khởi hành {departureDate.DepartureDate1:dd/MM/yyyy} cho tour '{tour.Title}' đã được cập nhật.",
                StatusConstants.NotificationType.DepartureDateUpdated,
                departureDate.Id.ToString()
            );

            // Send notification and email to all users with bookings
            foreach (var user in bookingUsers)
            {
                // Send notification
                await _notificationService.CreateNotificationAsync(
                    user.UserId,
                    "Ngày khởi hành tour đã được cập nhật",
                    $"Ngày khởi hành tour '{tour.Title}' đã được cập nhật thành {departureDate.DepartureDate1:dd/MM/yyyy}.",
                    StatusConstants.NotificationType.DepartureDateUpdated,
                    departureDate.Id.ToString()
                );

                // Send email
                if (!string.IsNullOrEmpty(user.Email))
                {
                    var emailSubject = "Ngày khởi hành tour đã được cập nhật";
                    var emailBody = $@"
                        <h3>Thông báo cập nhật ngày khởi hành</h3>
                        <p>Xin chào {user.UserName},</p>
                        <p>Ngày khởi hành tour '{tour.Title}' đã được cập nhật:</p>
                        <ul>
                            <li><strong>Ngày khởi hành mới:</strong> {departureDate.DepartureDate1:dd/MM/yyyy}</li>
                            <li><strong>Thời gian cập nhật:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</li>
                        </ul>
                        <p>Vui lòng kiểm tra lại thông tin và chuẩn bị cho chuyến đi.</p>
                        <p>Trân trọng,<br>Hệ thống quản lý tour</p>";

                    await _emailHelper.SendEmailAsync(user.Email, emailSubject, emailBody);
                }
            }

            // Send email to Tour Operator
            var tourOperatorEmail = tour.TourOperator.User.Email;
            if (!string.IsNullOrEmpty(tourOperatorEmail))
            {
                var emailSubject = "Ngày khởi hành đã được cập nhật";
                var emailBody = $@"
                    <h3>Thông báo cập nhật ngày khởi hành</h3>
                    <p>Xin chào {tour.TourOperator.User.UserName},</p>
                    <p>Ngày khởi hành đã được cập nhật thành công:</p>
                    <ul>
                        <li><strong>Tour:</strong> {tour.Title}</li>
                        <li><strong>Ngày khởi hành mới:</strong> {departureDate.DepartureDate1:dd/MM/yyyy}</li>
                        <li><strong>Thời gian cập nhật:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</li>
                        <li><strong>Số lượng booking bị ảnh hưởng:</strong> {bookingUsers.Count}</li>
                    </ul>
                    <p>Trân trọng,<br>Hệ thống quản lý tour</p>";

                await _emailHelper.SendEmailAsync(tourOperatorEmail, emailSubject, emailBody);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't throw to avoid breaking the main operation
            Console.WriteLine($"Error sending departure date updated notifications: {ex.Message}");
        }
    }

    public async Task SendDepartureDateCancelledNotificationsAsync(DepartureDate departureDate, int tourOperatorId)
    {
        try
        {
            // Get tour information
            var tour = await _context.Tours
                .Include(t => t.TourOperator)
                .ThenInclude(to => to.User)
                .FirstOrDefaultAsync(t => t.TourId == departureDate.TourId);

            if (tour == null) return;

            // Get all users with bookings for this departure date
            var bookingUsers = await _context.Bookings
                .Include(b => b.User)
                .Where(b => b.DepartureDateId == departureDate.Id && b.IsActive)
                .Select(b => b.User)
                .Distinct()
                .ToListAsync();

            // Send notification to Tour Operator
            await _notificationService.CreateNotificationAsync(
                tourOperatorId,
                "Ngày khởi hành đã được hủy",
                $"Ngày khởi hành {departureDate.DepartureDate1:dd/MM/yyyy} cho tour '{tour.Title}' đã được hủy.",
                StatusConstants.NotificationType.DepartureDateCancelled,
                departureDate.Id.ToString()
            );

            // Send notification and email to all users with bookings
            foreach (var user in bookingUsers)
            {
                // Send notification
                await _notificationService.CreateNotificationAsync(
                    user.UserId,
                    "Ngày khởi hành tour đã bị hủy",
                    $"Ngày khởi hành tour '{tour.Title}' vào {departureDate.DepartureDate1:dd/MM/yyyy} đã bị hủy. Booking của bạn cũng đã bị hủy.",
                    StatusConstants.NotificationType.DepartureDateCancelled,
                    departureDate.Id.ToString()
                );

                // Send email
                if (!string.IsNullOrEmpty(user.Email))
                {
                    var emailSubject = "Ngày khởi hành tour đã bị hủy";
                    var emailBody = $@"
                        <h3>Thông báo hủy ngày khởi hành</h3>
                        <p>Xin chào {user.UserName},</p>
                        <p>Chúng tôi rất tiếc phải thông báo rằng ngày khởi hành tour '{tour.Title}' đã bị hủy:</p>
                        <ul>
                            <li><strong>Ngày khởi hành bị hủy:</strong> {departureDate.DepartureDate1:dd/MM/yyyy}</li>
                            <li><strong>Thời gian hủy:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</li>
                        </ul>
                        <p>Booking của bạn cũng đã bị hủy. Chúng tôi sẽ liên hệ với bạn để xử lý hoàn tiền hoặc đổi ngày khác.</p>
                        <p>Xin lỗi vì sự bất tiện này.</p>
                        <p>Trân trọng,<br>Hệ thống quản lý tour</p>";

                    await _emailHelper.SendEmailAsync(user.Email, emailSubject, emailBody);
                }
            }

            // Send email to Tour Operator
            var tourOperatorEmail = tour.TourOperator.User.Email;
            if (!string.IsNullOrEmpty(tourOperatorEmail))
            {
                var emailSubject = "Ngày khởi hành đã được hủy";
                var emailBody = $@"
                    <h3>Thông báo hủy ngày khởi hành</h3>
                    <p>Xin chào {tour.TourOperator.User.UserName},</p>
                    <p>Ngày khởi hành đã được hủy thành công:</p>
                    <ul>
                        <li><strong>Tour:</strong> {tour.Title}</li>
                        <li><strong>Ngày khởi hành bị hủy:</strong> {departureDate.DepartureDate1:dd/MM/yyyy}</li>
                        <li><strong>Thời gian hủy:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</li>
                        <li><strong>Số lượng booking bị ảnh hưởng:</strong> {bookingUsers.Count}</li>
                    </ul>
                    <p>Lưu ý: Tất cả booking trong ngày khởi hành này đã được tự động hủy.</p>
                    <p>Trân trọng,<br>Hệ thống quản lý tour</p>";

                await _emailHelper.SendEmailAsync(tourOperatorEmail, emailSubject, emailBody);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't throw to avoid breaking the main operation
            Console.WriteLine($"Error sending departure date cancelled notifications: {ex.Message}");
        }
    }
} 