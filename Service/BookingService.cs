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
                    SelectedDepartureDate = x.SelectedDepartureDate,
                    BookingDate = x.BookingDate,
                    NumberOfAdults = x.NumberOfAdults,
                    NumberOfChildren = x.NumberOfChildren,
                    NumberOfInfants = x.NumberOfInfants,
                    NoteForTour = x.NoteForTour,
                    TotalPrice = x.TotalPrice,
                    DepositAmount = x.DepositAmount,
                    RemainingAmount = x.RemainingAmount,
                    Contract = x.Contract,
                    BookingStatus = x.BookingStatus,
                    PaymentStatus = x.PaymentStatus,
                    IsActive = x.IsActive,
                    UserName = x.User != null ? x.User.UserName : null,
                    TourTitle = x.Tour != null ? x.Tour.Title : null,
                    CompanyName = x.Tour != null && x.Tour.TourOperator != null ? x.Tour.TourOperator.CompanyName : null
                }).ToList()
            };
        }

        public async Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request)
        {
            var booking = new Booking
            {
                UserId = request.UserId,
                TourId = request.TourId,
                SelectedDepartureDate = request.SelectedDepartureDate,
                NumberOfAdults = request.NumberOfAdults,
                NumberOfChildren = request.NumberOfChildren,
                NumberOfInfants = request.NumberOfInfants,
                NoteForTour = request.NoteForTour,
                TotalPrice = request.TotalPrice,
                DepositAmount = request.DepositAmount,
                RemainingAmount = request.RemainingAmount,
                Contract = request.Contract,
                BookingStatus = "Pending",
                PaymentStatus = "Pending",
                IsActive = true
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Tạo notification khi booking thành công
            await _notificationService.CreateBookingSuccessNotificationAsync(request.UserId, booking.BookingId);

            return new BookingResponse
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                TourId = booking.TourId,
                SelectedDepartureDate = booking.SelectedDepartureDate,
                BookingDate = booking.BookingDate,
                NumberOfAdults = booking.NumberOfAdults,
                NumberOfChildren = booking.NumberOfChildren,
                NumberOfInfants = booking.NumberOfInfants,
                NoteForTour = booking.NoteForTour,
                TotalPrice = booking.TotalPrice,
                DepositAmount = booking.DepositAmount,
                RemainingAmount = booking.RemainingAmount,
                Contract = booking.Contract,
                BookingStatus = booking.BookingStatus,
                PaymentStatus = booking.PaymentStatus,
                IsActive = booking.IsActive,
                UserName = booking.User != null ? booking.User.UserName : null,
                TourTitle = booking.Tour != null ? booking.Tour.Title : null,
                CompanyName = booking.Tour != null && booking.Tour.TourOperator != null ? booking.Tour.TourOperator.CompanyName : null
            };
        }

        public async Task<BookingResponse> UpdateBookingAsync(UpdateBookingRequest request)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == request.BookingId);
            if (booking == null) return null;
            if (request.SelectedDepartureDate.HasValue)
                booking.SelectedDepartureDate = request.SelectedDepartureDate.Value;
            if (request.NumberOfAdults.HasValue)
                booking.NumberOfAdults = request.NumberOfAdults;
            if (request.NumberOfChildren.HasValue)
                booking.NumberOfChildren = request.NumberOfChildren;
            if (request.NumberOfInfants.HasValue)
                booking.NumberOfInfants = request.NumberOfInfants;
            if (request.NoteForTour != null)
                booking.NoteForTour = request.NoteForTour;
            if (request.TotalPrice.HasValue)
                booking.TotalPrice = request.TotalPrice;
            if (request.DepositAmount.HasValue)
                booking.DepositAmount = request.DepositAmount;
            if (request.RemainingAmount.HasValue)
                booking.RemainingAmount = request.RemainingAmount;
            if (request.Contract != null)
                booking.Contract = request.Contract;
            if (request.BookingStatus != null)
                booking.BookingStatus = request.BookingStatus;
            if (request.PaymentStatus != null)
                booking.PaymentStatus = request.PaymentStatus;
            await _context.SaveChangesAsync();
            return new BookingResponse
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                TourId = booking.TourId,
                SelectedDepartureDate = booking.SelectedDepartureDate,
                BookingDate = booking.BookingDate,
                NumberOfAdults = booking.NumberOfAdults,
                NumberOfChildren = booking.NumberOfChildren,
                NumberOfInfants = booking.NumberOfInfants,
                NoteForTour = booking.NoteForTour,
                TotalPrice = booking.TotalPrice,
                DepositAmount = booking.DepositAmount,
                RemainingAmount = booking.RemainingAmount,
                Contract = booking.Contract,
                BookingStatus = booking.BookingStatus,
                PaymentStatus = booking.PaymentStatus,
                IsActive = booking.IsActive,
                UserName = booking.User != null ? booking.User.UserName : null,
                TourTitle = booking.Tour != null ? booking.Tour.Title : null,
                CompanyName = booking.Tour != null && booking.Tour.TourOperator != null ? booking.Tour.TourOperator.CompanyName : null
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

        // New methods for role-based booking retrieval
        public async Task<BookingListResponse> GetCustomerBookingsAsync(BookingSearchRequest request)
        {
            var userId = int.Parse(System.Security.Claims.ClaimsPrincipal.Current?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var query = _context.Bookings
                .Include(x => x.Tour)
                .ThenInclude(t => t.TourOperator)
                .Include(x => x.User)
                .Where(x => x.UserId == userId && x.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();
                query = query.Where(x =>
                    (x.User != null && x.User.UserName.ToLower().Contains(keyword)) ||
                    (x.Tour != null && x.Tour.Title.ToLower().Contains(keyword)) ||
                    (x.Tour != null && x.Tour.TourOperator != null && x.Tour.TourOperator.CompanyName != null && x.Tour.TourOperator.CompanyName.ToLower().Contains(keyword))
                );
            }

            var bookings = await query.ToListAsync();
            return new BookingListResponse
            {
                Bookings = bookings.Select(x => new BookingResponse
                {
                    BookingId = x.BookingId,
                    UserId = x.UserId,
                    TourId = x.TourId,
                    SelectedDepartureDate = x.SelectedDepartureDate,
                    BookingDate = x.BookingDate,
                    NumberOfAdults = x.NumberOfAdults,
                    NumberOfChildren = x.NumberOfChildren,
                    NumberOfInfants = x.NumberOfInfants,
                    NoteForTour = x.NoteForTour,
                    TotalPrice = x.TotalPrice,
                    DepositAmount = x.DepositAmount,
                    RemainingAmount = x.RemainingAmount,
                    Contract = x.Contract,
                    BookingStatus = x.BookingStatus,
                    PaymentStatus = x.PaymentStatus,
                    IsActive = x.IsActive,
                    UserName = x.User != null ? x.User.UserName : null,
                    TourTitle = x.Tour != null ? x.Tour.Title : null,
                    CompanyName = x.Tour != null && x.Tour.TourOperator != null ? x.Tour.TourOperator.CompanyName : null
                }).ToList()
            };
        }

        public async Task<BookingListResponse> GetTourOperatorBookingsAsync(BookingSearchRequest request)
        {
            var userId = int.Parse(System.Security.Claims.ClaimsPrincipal.Current?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var tourOperatorId = _context.Users.Include(u => u.TourOperator).FirstOrDefault(u => u.UserId == userId)?.TourOperator?.TourOperatorId;
            var query = _context.Bookings
                .Include(x => x.Tour)
                .ThenInclude(t => t.TourOperator)
                .Include(x => x.User)
                .Where(x => x.Tour.TourOperatorId == tourOperatorId && x.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();
                query = query.Where(x =>
                    (x.User != null && x.User.UserName.ToLower().Contains(keyword)) ||
                    (x.Tour != null && x.Tour.Title.ToLower().Contains(keyword)) ||
                    (x.Tour != null && x.Tour.TourOperator != null && x.Tour.TourOperator.CompanyName != null && x.Tour.TourOperator.CompanyName.ToLower().Contains(keyword))
                );
            }

            var bookings = await query.ToListAsync();
            return new BookingListResponse
            {
                Bookings = bookings.Select(x => new BookingResponse
                {
                    BookingId = x.BookingId,
                    UserId = x.UserId,
                    TourId = x.TourId,
                    SelectedDepartureDate = x.SelectedDepartureDate,
                    BookingDate = x.BookingDate,
                    NumberOfAdults = x.NumberOfAdults,
                    NumberOfChildren = x.NumberOfChildren,
                    NumberOfInfants = x.NumberOfInfants,
                    NoteForTour = x.NoteForTour,
                    TotalPrice = x.TotalPrice,
                    DepositAmount = x.DepositAmount,
                    RemainingAmount = x.RemainingAmount,
                    Contract = x.Contract,
                    BookingStatus = x.BookingStatus,
                    PaymentStatus = x.PaymentStatus,
                    IsActive = x.IsActive,
                    UserName = x.User != null ? x.User.UserName : null,
                    TourTitle = x.Tour != null ? x.Tour.Title : null,
                    CompanyName = x.Tour != null && x.Tour.TourOperator != null ? x.Tour.TourOperator.CompanyName : null
                }).ToList()
            };
        }

        public async Task<BookingListResponse> GetAllBookingsForAdminAsync(BookingSearchRequest request)
        {
            var query = _context.Bookings
                .Include(x => x.Tour)
                .ThenInclude(t => t.TourOperator)
                .Include(x => x.User)
                .Where(x => x.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();
                query = query.Where(x =>
                    (x.User != null && x.User.UserName.ToLower().Contains(keyword)) ||
                    (x.Tour != null && x.Tour.Title.ToLower().Contains(keyword)) ||
                    (x.Tour != null && x.Tour.TourOperator != null && x.Tour.TourOperator.CompanyName != null && x.Tour.TourOperator.CompanyName.ToLower().Contains(keyword))
                );
            }

            var bookings = await query.ToListAsync();
            return new BookingListResponse
            {
                Bookings = bookings.Select(x => new BookingResponse
                {
                    BookingId = x.BookingId,
                    UserId = x.UserId,
                    TourId = x.TourId,
                    SelectedDepartureDate = x.SelectedDepartureDate,
                    BookingDate = x.BookingDate,
                    NumberOfAdults = x.NumberOfAdults,
                    NumberOfChildren = x.NumberOfChildren,
                    NumberOfInfants = x.NumberOfInfants,
                    NoteForTour = x.NoteForTour,
                    TotalPrice = x.TotalPrice,
                    DepositAmount = x.DepositAmount,
                    RemainingAmount = x.RemainingAmount,
                    Contract = x.Contract,
                    BookingStatus = x.BookingStatus,
                    PaymentStatus = x.PaymentStatus,
                    IsActive = x.IsActive,
                    UserName = x.User != null ? x.User.UserName : null,
                    TourTitle = x.Tour != null ? x.Tour.Title : null,
                    CompanyName = x.Tour != null && x.Tour.TourOperator != null ? x.Tour.TourOperator.CompanyName : null
                }).ToList()
            };
        }
    }
} 