using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.DepartureDatesRequest;
using TourManagement_BE.Data.DTO.Response.DepartureDateResponse;
using TourManagement_BE.Data.Models;

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
                    BookingDate = b.BookingDate,
                    NumberOfAdults = b.NumberOfAdults,
                    NumberOfChildren = b.NumberOfChildren,
                    NumberOfInfants = b.NumberOfInfants,
                    TotalPrice = b.TotalPrice,
                    BookingStatus = b.BookingStatus,
                    PaymentStatus = b.PaymentStatus
                }).ToList()
            })
            .ToListAsync();

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

        return departureDates;
    }
} 