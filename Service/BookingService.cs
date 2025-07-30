using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Helper.Constant;

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
            var query = _context.Bookings
                .Include(x => x.Tour).ThenInclude(t => t.TourOperator)
                .Include(x => x.User)
                .AsQueryable();

            // Search by Tour Name
            if (!string.IsNullOrEmpty(request.TourName))
            {
                query = query.Where(x => x.Tour != null && x.Tour.Title.Contains(request.TourName));
            }

            // Search by User Name
            if (!string.IsNullOrEmpty(request.UserName))
            {
                query = query.Where(x => x.User != null && x.User.UserName.Contains(request.UserName));
            }

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
                BookingStatus = StatusConstants.Booking.Pending,
                PaymentStatus = StatusConstants.Payment.Pending,
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
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Tour).ThenInclude(t => t.TourOperator)
                .FirstOrDefaultAsync(x => x.BookingId == request.BookingId);
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
            var query = _context.Bookings
                .Include(x => x.Tour).ThenInclude(t => t.TourOperator)
                .Include(x => x.User)
                .Where(x => x.UserId == userId && x.IsActive);

            // Search by Tour Name
            if (!string.IsNullOrEmpty(request.TourName))
            {
                query = query.Where(x => x.Tour != null && x.Tour.Title.Contains(request.TourName));
            }

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

        public async Task<BookingListResponse> GetTourOperatorBookingsAsync(BookingSearchRequest request, int userId)
        {
            // Tìm TourOperatorId từ userId
            var tourOperator = await _context.TourOperators
                .FirstOrDefaultAsync(to => to.UserId == userId);
            
            if (tourOperator == null)
            {
                return new BookingListResponse { Bookings = new List<BookingResponse>() };
            }
            
            var query = _context.Bookings
                .Include(x => x.Tour).ThenInclude(t => t.TourOperator)
                .Include(x => x.User)
                .Where(x => x.Tour != null && x.Tour.TourOperatorId == tourOperator.TourOperatorId && x.IsActive);

            // Search by Tour Name
            if (!string.IsNullOrEmpty(request.TourName))
            {
                query = query.Where(x => x.Tour != null && x.Tour.Title.Contains(request.TourName));
            }

            // Search by User Name
            if (!string.IsNullOrEmpty(request.UserName))
            {
                query = query.Where(x => x.User != null && x.User.UserName.Contains(request.UserName));
            }

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

        public async Task<BookingListResponse> GetAllBookingsForAdminAsync(BookingSearchRequest request)
        {
            var query = _context.Bookings
                .Include(x => x.Tour).ThenInclude(t => t.TourOperator)
                .Include(x => x.User)
                .Where(x => x.IsActive);

            // Search by Tour Name
            if (!string.IsNullOrEmpty(request.TourName))
            {
                query = query.Where(x => x.Tour != null && x.Tour.Title.Contains(request.TourName));
            }

            // Search by User Name
            if (!string.IsNullOrEmpty(request.UserName))
            {
                query = query.Where(x => x.User != null && x.User.UserName.Contains(request.UserName));
            }

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

        // New detailed booking methods
        public async Task<BookingDetailListResponse> GetBookingsDetailedAsync(BookingSearchRequest request)
        {
            var query = _context.Bookings
                .Include(x => x.Tour).ThenInclude(t => t.TourOperator)
                .Include(x => x.User)
                .Include(x => x.DepartureDate)
                .AsQueryable();

            // Search by Tour Name
            if (!string.IsNullOrEmpty(request.TourName))
            {
                query = query.Where(x => x.Tour != null && x.Tour.Title.Contains(request.TourName));
            }

            // Search by User Name
            if (!string.IsNullOrEmpty(request.UserName))
            {
                query = query.Where(x => x.User != null && x.User.UserName.Contains(request.UserName));
            }

            var bookings = await query.ToListAsync();
            return new BookingDetailListResponse
            {
                Bookings = bookings.Select(x => MapToBookingDetailResponse(x)).ToList()
            };
        }

        public async Task<BookingDetailListResponse> GetCustomerBookingsDetailedAsync(BookingSearchRequest request, int userId)
        {
            var query = _context.Bookings
                .Include(x => x.Tour).ThenInclude(t => t.TourOperator)
                .Include(x => x.User)
                .Include(x => x.DepartureDate)
                .Where(x => x.UserId == userId && x.IsActive);

            // Search by Tour Name
            if (!string.IsNullOrEmpty(request.TourName))
            {
                query = query.Where(x => x.Tour != null && x.Tour.Title.Contains(request.TourName));
            }

            var bookings = await query.ToListAsync();
            return new BookingDetailListResponse
            {
                Bookings = bookings.Select(x => MapToBookingDetailResponse(x)).ToList()
            };
        }

        public async Task<BookingDetailListResponse> GetTourOperatorBookingsDetailedAsync(BookingSearchRequest request, int userId)
        {
            // Tìm TourOperatorId từ userId
            var tourOperator = await _context.TourOperators
                .FirstOrDefaultAsync(to => to.UserId == userId);
            
            if (tourOperator == null)
            {
                return new BookingDetailListResponse { Bookings = new List<BookingDetailResponse>() };
            }
            
            var query = _context.Bookings
                .Include(x => x.Tour).ThenInclude(t => t.TourOperator)
                .Include(x => x.User)
                .Include(x => x.DepartureDate)
                .Where(x => x.Tour != null && x.Tour.TourOperatorId == tourOperator.TourOperatorId && x.IsActive);

            // Search by Tour Name
            if (!string.IsNullOrEmpty(request.TourName))
            {
                query = query.Where(x => x.Tour != null && x.Tour.Title.Contains(request.TourName));
            }

            // Search by User Name
            if (!string.IsNullOrEmpty(request.UserName))
            {
                query = query.Where(x => x.User != null && x.User.UserName.Contains(request.UserName));
            }

            var bookings = await query.ToListAsync();
            
            return new BookingDetailListResponse
            {
                Bookings = bookings.Select(x => MapToBookingDetailResponse(x)).ToList()
            };
        }

        public async Task<BookingDetailListResponse> GetAllBookingsForAdminDetailedAsync(BookingSearchRequest request)
        {
            var query = _context.Bookings
                .Include(x => x.Tour).ThenInclude(t => t.TourOperator)
                .Include(x => x.User)
                .Include(x => x.DepartureDate)
                .Where(x => x.IsActive);

            // Search by Tour Name
            if (!string.IsNullOrEmpty(request.TourName))
            {
                query = query.Where(x => x.Tour != null && x.Tour.Title.Contains(request.TourName));
            }

            // Search by User Name
            if (!string.IsNullOrEmpty(request.UserName))
            {
                query = query.Where(x => x.User != null && x.User.UserName.Contains(request.UserName));
            }

            var bookings = await query.ToListAsync();
            return new BookingDetailListResponse
            {
                Bookings = bookings.Select(x => MapToBookingDetailResponse(x)).ToList()
            };
        }

        private BookingDetailResponse MapToBookingDetailResponse(Booking booking)
        {
            return new BookingDetailResponse
            {
                BookingId = booking.BookingId,
                Tour = new TourInfo
                {
                    Title = booking.Tour?.Title ?? string.Empty,
                    MaxSlots = booking.Tour?.MaxSlots ?? 0,
                    Transportation = booking.Tour?.Transportation,
                    StartPoint = booking.Tour?.StartPoint,
                    DepartureDate = booking.DepartureDate.DepartureDate1
                },
                Booking = new BookingInfo
                {
                    BookingDate = booking.BookingDate,
                    Contract = booking.Contract,
                    NoteForTour = booking.NoteForTour
                },
                Guest = new GuestInfo
                {
                    NumberOfAdults = booking.NumberOfAdults ?? 0,
                    NumberOfChildren = booking.NumberOfChildren ?? 0,
                    NumberOfInfants = booking.NumberOfInfants ?? 0
                },
                BillingInfo = new BillingInfo
                {
                    Username = booking.User?.UserName,
                    Email = booking.User?.Email,
                    Phone = booking.User?.PhoneNumber,
                    Address = booking.User?.Address
                },
                PaymentInfo = new PaymentInfo
                {
                    TotalPrice = booking.TotalPrice,
                    PaymentStatus = booking.PaymentStatus,
                    BookingStatus = booking.BookingStatus
                }
            };
        }

        public async Task<BookingResponse> UpdatePaymentStatusAsync(UpdatePaymentStatusRequest request, int tourOperatorId)
        {
            // Validate booking exists and belongs to this tour operator
            var booking = await _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BookingId == request.BookingId && b.IsActive);

            if (booking == null)
                throw new Exception("Booking not found");

            if (booking.Tour?.TourOperatorId != tourOperatorId)
                throw new Exception("You don't have permission to update this booking");

            // Validate payment status
            if (!StatusConstants.Payment.ValidStatuses.Contains(request.PaymentStatus))
                throw new Exception("Invalid payment status");

            // Update booking payment status
            booking.PaymentStatus = request.PaymentStatus;

            // Update related payments
            var payments = await _context.Payments
                .Where(p => p.BookingId == request.BookingId && p.IsActive)
                .ToListAsync();

            foreach (var payment in payments)
            {
                payment.PaymentStatus = request.PaymentStatus;
                if (request.PaymentStatus == StatusConstants.Payment.Paid && payment.AmountPaid == 0)
                {
                    payment.AmountPaid = payment.Amount;
                    payment.PaymentDate = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();

            // Send notification to customer
            await _notificationService.CreateNotificationAsync(
                booking.UserId,
                "Payment Status Updated",
                $"Your payment status for booking #{booking.BookingId} has been updated to {request.PaymentStatus}",
                "Payment",
                booking.BookingId.ToString()
            );

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
                UserName = booking.User?.UserName,
                TourTitle = booking.Tour?.Title,
                CompanyName = booking.Tour?.TourOperator?.CompanyName,
                TourOperatorId = booking.Tour?.TourOperatorId
            };
        }

        public async Task<BookingResponse> UpdateBookingStatusAsync(UpdateBookingStatusRequest request, int tourOperatorId)
        {
            // Validate booking exists and belongs to this tour operator
            var booking = await _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BookingId == request.BookingId && b.IsActive);

            if (booking == null)
                throw new Exception("Booking not found");

            if (booking.Tour?.TourOperatorId != tourOperatorId)
                throw new Exception("You don't have permission to update this booking");

            // Validate booking status
            if (!StatusConstants.Booking.ValidStatuses.Contains(request.BookingStatus))
                throw new Exception("Invalid booking status");

            // Update booking status
            booking.BookingStatus = request.BookingStatus;

            await _context.SaveChangesAsync();

            // Send notification to customer
            await _notificationService.CreateNotificationAsync(
                booking.UserId,
                "Booking Status Updated",
                $"Your booking #{booking.BookingId} status has been updated to {request.BookingStatus}",
                "Booking",
                booking.BookingId.ToString()
            );

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
                UserName = booking.User?.UserName,
                TourTitle = booking.Tour?.Title,
                CompanyName = booking.Tour?.TourOperator?.CompanyName,
                TourOperatorId = booking.Tour?.TourOperatorId
            };
        }
    }
} 