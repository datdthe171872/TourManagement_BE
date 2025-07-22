using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Service
{
    public class BookingService : IBookingService
    {
        private readonly MyDBContext _context;
        private readonly INotificationService _notificationService;

        public BookingService(MyDBContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<BookingListResponse> GetBookingsAsync(BookingSearchRequest request)
        {
            var query = _context.Bookings.AsQueryable();          
            var bookings = await query.ToListAsync();
            return new BookingListResponse
            {
                Bookings = bookings.Select(x => new BookingResponse
                {
                    BookingId = x.BookingId,
                    UserId = x.UserId,
                    TourId = x.TourId,
                    DepartureDateId = x.DepartureDateId,
                    BookingDate = x.BookingDate,
                    NumberOfAdults = x.NumberOfAdults ?? 0,
                    NumberOfChildren = x.NumberOfChildren ?? 0,
                    NumberOfInfants = x.NumberOfInfants ?? 0,
                    NoteForTour = x.NoteForTour,
                    TotalPrice = x.TotalPrice,
                    Contract = x.Contract,
                    BookingStatus = x.BookingStatus,
                    PaymentStatus = x.PaymentStatus,
                    IsActive = x.IsActive,
                    UserName = x.User != null ? x.User.UserName : null,
                    TourTitle = x.Tour != null ? x.Tour.Title : null,
                    CompanyName = x.Tour != null && x.Tour.TourOperator != null ? x.Tour.TourOperator.CompanyName : null,
                    TourOperatorId = x.Tour != null ? x.Tour.TourOperatorId : (int?)null
                }).ToList()
            };
        }

        public async Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request, int userId)
        {
            var tour = await _context.Tours.FirstOrDefaultAsync(t => t.TourId == request.TourId);
            if (tour == null)
                throw new Exception("Tour not found");

            var departure = await _context.DepartureDates.FirstOrDefaultAsync(d => d.Id == request.DepartureDateId && d.TourId == request.TourId && d.IsActive);
            if (departure == null)
                throw new Exception("Departure date not found or not active for this tour");

            int totalPeople = request.NumberOfAdults + request.NumberOfChildren + request.NumberOfInfants;
            int slotsBooked = tour.SlotsBooked ?? 0;
            int availableSlots = tour.MaxSlots - slotsBooked;
            if (totalPeople > availableSlots)
                throw new Exception($"Not enough slots. Available: {availableSlots}, Requested: {totalPeople}");

            decimal totalPrice = request.NumberOfAdults * tour.PriceOfAdults
                               + request.NumberOfChildren * tour.PriceOfChildren
                               + request.NumberOfInfants * tour.PriceOfInfants;

            var booking = new Booking
            {
                UserId = userId,
                TourId = request.TourId,
                DepartureDateId = request.DepartureDateId,
                NumberOfAdults = request.NumberOfAdults,
                NumberOfChildren = request.NumberOfChildren,
                NumberOfInfants = request.NumberOfInfants,
                NoteForTour = request.NoteForTour,
                TotalPrice = totalPrice,
                BookingStatus = "Pending",
                PaymentStatus = "Pending",
                IsActive = true
            };
            _context.Bookings.Add(booking);

            // Update SlotsBooked
            tour.SlotsBooked = slotsBooked + totalPeople;
            await _context.SaveChangesAsync();

            await _notificationService.CreateBookingSuccessNotificationAsync(userId, booking.BookingId);

            // Truy vấn lại booking với navigation property
            var bookingWithNav = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Tour)
                    .ThenInclude(t => t.TourOperator)
                .FirstOrDefaultAsync(b => b.BookingId == booking.BookingId);

            return new BookingResponse
            {
                BookingId = bookingWithNav.BookingId,
                UserId = bookingWithNav.UserId,
                TourId = bookingWithNav.TourId,
                DepartureDateId = bookingWithNav.DepartureDateId,
                BookingDate = bookingWithNav.BookingDate,
                NumberOfAdults = bookingWithNav.NumberOfAdults ?? 0,
                NumberOfChildren = bookingWithNav.NumberOfChildren ?? 0,
                NumberOfInfants = bookingWithNav.NumberOfInfants ?? 0,
                NoteForTour = bookingWithNav.NoteForTour,
                TotalPrice = bookingWithNav.TotalPrice,
                Contract = bookingWithNav.Contract,
                BookingStatus = bookingWithNav.BookingStatus,
                PaymentStatus = bookingWithNav.PaymentStatus,
                IsActive = bookingWithNav.IsActive,
                UserName = bookingWithNav.User != null ? bookingWithNav.User.UserName : null,
                TourTitle = bookingWithNav.Tour != null ? bookingWithNav.Tour.Title : null,
                CompanyName = bookingWithNav.Tour != null && bookingWithNav.Tour.TourOperator != null ? bookingWithNav.Tour.TourOperator.CompanyName : null,
                TourOperatorId = bookingWithNav.Tour != null ? bookingWithNav.Tour.TourOperatorId : (int?)null
            };
        }

        public async Task<BookingResponse> UpdateBookingAsync(UpdateBookingRequest request)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == request.BookingId);
            if (booking == null) return null;
            // Không update BookingStatus, PaymentStatus nữa
            booking.DepartureDateId = request.DepartureDateId;
            booking.NumberOfAdults = request.NumberOfAdults;
            booking.NumberOfChildren = request.NumberOfChildren;
            booking.NumberOfInfants = request.NumberOfInfants;
            if (request.NoteForTour != null)
                booking.NoteForTour = request.NoteForTour;
            if (request.Contract != null)
                booking.Contract = request.Contract;
            await _context.SaveChangesAsync();
            return new BookingResponse
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                TourId = booking.TourId,
                DepartureDateId = booking.DepartureDateId,
                BookingDate = booking.BookingDate,
                NumberOfAdults = booking.NumberOfAdults ?? 0,
                NumberOfChildren = booking.NumberOfChildren ?? 0,
                NumberOfInfants = booking.NumberOfInfants ?? 0,
                NoteForTour = booking.NoteForTour,
                TotalPrice = booking.TotalPrice,
                Contract = booking.Contract,
                BookingStatus = booking.BookingStatus,
                PaymentStatus = booking.PaymentStatus,
                IsActive = booking.IsActive,
                UserName = booking.User != null ? booking.User.UserName : null,
                TourTitle = booking.Tour != null ? booking.Tour.Title : null,
                CompanyName = booking.Tour != null && booking.Tour.TourOperator != null ? booking.Tour.TourOperator.CompanyName : null,
                TourOperatorId = booking.Tour != null ? booking.Tour.TourOperatorId : (int?)null
            };
        }

        public async Task<bool> SoftDeleteBookingAsync(int bookingId, int userId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == bookingId && x.UserId == userId);
            if (booking == null) return false;
            booking.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BookingResponse> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Tour).ThenInclude(t => t.TourOperator)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null) return null;
            return new BookingResponse
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                TourId = booking.TourId,
                DepartureDateId = booking.DepartureDateId,
                BookingDate = booking.BookingDate,
                NumberOfAdults = booking.NumberOfAdults ?? 0,
                NumberOfChildren = booking.NumberOfChildren ?? 0,
                NumberOfInfants = booking.NumberOfInfants ?? 0,
                NoteForTour = booking.NoteForTour,
                TotalPrice = booking.TotalPrice,
                Contract = booking.Contract,
                BookingStatus = booking.BookingStatus,
                PaymentStatus = booking.PaymentStatus,
                IsActive = booking.IsActive,
                UserName = booking.User != null ? booking.User.UserName : null,
                TourTitle = booking.Tour != null ? booking.Tour.Title : null,
                CompanyName = booking.Tour != null && booking.Tour.TourOperator != null ? booking.Tour.TourOperator.CompanyName : null,
                TourOperatorId = booking.Tour != null ? booking.Tour.TourOperatorId : (int?)null
            };
        }

        public async Task<BookingResponse> UpdateBookingContractAsync(UpdateBookingRequest request)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Tour).ThenInclude(t => t.TourOperator)
                .FirstOrDefaultAsync(x => x.BookingId == request.BookingId);
            if (booking == null) return null;
            booking.Contract = request.Contract;
            await _context.SaveChangesAsync();
            return new BookingResponse
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                TourId = booking.TourId,
                DepartureDateId = booking.DepartureDateId,
                BookingDate = booking.BookingDate,
                NumberOfAdults = booking.NumberOfAdults ?? 0,
                NumberOfChildren = booking.NumberOfChildren ?? 0,
                NumberOfInfants = booking.NumberOfInfants ?? 0,
                NoteForTour = booking.NoteForTour,
                TotalPrice = booking.TotalPrice,
                Contract = booking.Contract,
                BookingStatus = booking.BookingStatus,
                PaymentStatus = booking.PaymentStatus,
                IsActive = booking.IsActive,
                UserName = booking.User != null ? booking.User.UserName : null,
                TourTitle = booking.Tour != null ? booking.Tour.Title : null,
                CompanyName = booking.Tour != null && booking.Tour.TourOperator != null ? booking.Tour.TourOperator.CompanyName : null,
                TourOperatorId = booking.Tour != null ? booking.Tour.TourOperatorId : (int?)null
            };
        }

        // New methods for role-based booking retrieval
        public async Task<BookingListResponse> GetCustomerBookingsAsync(BookingSearchRequest request, int userId)
        {
            var bookings = await _context.Bookings
                .Include(x => x.Tour).ThenInclude(t => t.TourOperator)
                .Include(x => x.User)
                .Where(x => x.UserId == userId && x.IsActive)
                .ToListAsync();
            return new BookingListResponse
            {
                Bookings = bookings.Select(x => new BookingResponse
                {
                    BookingId = x.BookingId,
                    UserId = x.UserId,
                    TourId = x.TourId,
                    DepartureDateId = x.DepartureDateId,
                    BookingDate = x.BookingDate,
                    NumberOfAdults = x.NumberOfAdults ?? 0,
                    NumberOfChildren = x.NumberOfChildren ?? 0,
                    NumberOfInfants = x.NumberOfInfants ?? 0,
                    NoteForTour = x.NoteForTour,
                    TotalPrice = x.TotalPrice,
                    Contract = x.Contract,
                    BookingStatus = x.BookingStatus,
                    PaymentStatus = x.PaymentStatus,
                    UserName = x.User != null ? x.User.UserName : null,
                    TourTitle = x.Tour != null ? x.Tour.Title : null,
                    CompanyName = x.Tour != null && x.Tour.TourOperator != null ? x.Tour.TourOperator.CompanyName : null,
                    TourOperatorId = x.Tour != null ? x.Tour.TourOperatorId : (int?)null
                }).ToList()
            };
        }

        public async Task<BookingListResponse> GetTourOperatorBookingsAsync(BookingSearchRequest request)
        {
            // Lấy userId từ ClaimsPrincipal (chính là TourOperatorId)
            var userIdClaim = System.Security.Claims.ClaimsPrincipal.Current?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            int tourOperatorId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            var bookings = await _context.Bookings
                .Include(x => x.Tour).ThenInclude(t => t.TourOperator)
                .Include(x => x.User)
                .Where(x => x.Tour != null && x.Tour.TourOperatorId == tourOperatorId && x.IsActive)
                .ToListAsync();
            return new BookingListResponse
            {
                Bookings = bookings.Select(x => new BookingResponse
                {
                    BookingId = x.BookingId,
                    UserId = x.UserId,
                    TourId = x.TourId,
                    DepartureDateId = x.DepartureDateId,
                    BookingDate = x.BookingDate,
                    NumberOfAdults = x.NumberOfAdults ?? 0,
                    NumberOfChildren = x.NumberOfChildren ?? 0,
                    NumberOfInfants = x.NumberOfInfants ?? 0,
                    NoteForTour = x.NoteForTour,
                    TotalPrice = x.TotalPrice,
                    Contract = x.Contract,
                    BookingStatus = x.BookingStatus,
                    PaymentStatus = x.PaymentStatus,
                    UserName = x.User != null ? x.User.UserName : null,
                    TourTitle = x.Tour != null ? x.Tour.Title : null,
                    CompanyName = x.Tour != null && x.Tour.TourOperator != null ? x.Tour.TourOperator.CompanyName : null,
                    TourOperatorId = x.Tour != null ? x.Tour.TourOperatorId : (int?)null
                }).ToList()
            };
        }

        public async Task<BookingListResponse> GetAllBookingsForAdminAsync(BookingSearchRequest request)
        {
            var bookings = await _context.Bookings
                .Include(x => x.Tour).ThenInclude(t => t.TourOperator)
                .Include(x => x.User)
                .Where(x => x.IsActive)
                .ToListAsync();
            return new BookingListResponse
            {
                Bookings = bookings.Select(x => new BookingResponse
                {
                    BookingId = x.BookingId,
                    UserId = x.UserId,
                    TourId = x.TourId,
                    DepartureDateId = x.DepartureDateId,
                    BookingDate = x.BookingDate,
                    NumberOfAdults = x.NumberOfAdults ?? 0,
                    NumberOfChildren = x.NumberOfChildren ?? 0,
                    NumberOfInfants = x.NumberOfInfants ?? 0,
                    NoteForTour = x.NoteForTour,
                    TotalPrice = x.TotalPrice,
                    Contract = x.Contract,
                    BookingStatus = x.BookingStatus,
                    PaymentStatus = x.PaymentStatus,
                    UserName = x.User != null ? x.User.UserName : null,
                    TourTitle = x.Tour != null ? x.Tour.Title : null,
                    CompanyName = x.Tour != null && x.Tour.TourOperator != null ? x.Tour.TourOperator.CompanyName : null,
                    TourOperatorId = x.Tour != null ? x.Tour.TourOperatorId : (int?)null
                }).ToList()
            };
        }
    }
} 