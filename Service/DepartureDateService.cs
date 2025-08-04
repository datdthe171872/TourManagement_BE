using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.DepartureDatesRequest;
using TourManagement_BE.Data.DTO.Response.DepartureDateResponse;
using BookingResponse = TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Helper.Constant;

namespace TourManagement_BE.Service;

public class DepartureDateService : IDepartureDateService
{
    private readonly MyDBContext _context;

    public DepartureDateService(MyDBContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateDepartureDatesAsync(CreateDepartureDateRequest request)
    {
        // Kiểm tra tour có tồn tại không
        var tour = await _context.Tours
            .FirstOrDefaultAsync(t => t.TourId == request.TourId && t.IsActive);
        
        if (tour == null)
            return false;

        // Kiểm tra ngày bắt đầu không được trong quá khứ
        if (request.StartDate.Date <= DateTime.Now.Date)
            return false;

        // Parse DurationInDays để lấy số ngày
        if (!int.TryParse(tour.DurationInDays, out int durationInDays))
            return false;

        // Tự động tính số lượng ngày khởi hành = DurationInDays
        int numberOfDepartureDates = durationInDays;
        
        // Giới hạn tối đa 12 ngày khởi hành
        if (numberOfDepartureDates > 12)
            numberOfDepartureDates = 12;

        // Tính khoảng cách giữa các ngày khởi hành
        int daysBetweenDepartures = durationInDays + 1;

        var departureDates = new List<DepartureDate>();
        var currentDate = request.StartDate;

        for (int i = 0; i < numberOfDepartureDates; i++)
        {
            var departureDate = new DepartureDate
            {
                TourId = request.TourId,
                DepartureDate1 = currentDate,
                IsActive = true
            };

            departureDates.Add(departureDate);
            currentDate = currentDate.AddDays(daysBetweenDepartures);
        }

        await _context.DepartureDates.AddRangeAsync(departureDates);
        await _context.SaveChangesAsync();

        return true;
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
        var tourOperator = await _context.TourOperators
            .FirstOrDefaultAsync(to => to.UserId == userId && to.IsActive);
        if (tourOperator == null)
            return null;
        var departureDate = await _context.DepartureDates
            .Include(dd => dd.Tour)
            .Include(dd => dd.Bookings.Where(b => b.IsActive))
                .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(dd => dd.Id == departureDateId && 
                                     dd.Tour.TourOperatorId == tourOperator.TourOperatorId && 
                                     dd.IsActive);
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
        departureDate.IsActive = false;

        // Bước 5: Cập nhật trạng thái tất cả Booking trong DepartureDate này thành Cancelled
        foreach (var booking in departureDate.Bookings)
        {
            booking.BookingStatus = BookingStatus.Cancelled;
        }

        // Bước 6: Lưu thay đổi vào database
        await _context.SaveChangesAsync();

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
                        dd.IsCancelDate && 
                        !dd.IsActive)
            .OrderBy(dd => dd.DepartureDate1)
            .Select(dd => new DepartureDateResponse
            {
                Id = dd.Id,
                TourId = dd.TourId,
                TourTitle = dd.Tour.Title,
                DepartureDate = dd.DepartureDate1,
                IsActive = dd.IsActive,
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
                                     dd.IsCancelDate && 
                                     !dd.IsActive);

        if (departureDate == null)
            return false;

        // Bước 3: Kiểm tra ngày khởi hành phải cách hiện tại ít nhất 5 ngày
        var daysUntilDeparture = (departureDate.DepartureDate1.Date - DateTime.Now.Date).Days;
        if (daysUntilDeparture < 5)
            return false;

        // Bước 4: Cập nhật trạng thái DepartureDate thành active
        departureDate.IsCancelDate = false;
        departureDate.IsActive = true;

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
} 