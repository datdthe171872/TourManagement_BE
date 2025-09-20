using System;
using System.Collections.Generic;
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
        private readonly IEmailService _emailService;

        public BookingService(MyDBContext context, INotificationService notificationService, IEmailService emailService)
        {
            _context = context;
            _notificationService = notificationService;
            _emailService = emailService;
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
                    TourOperatorId = x.Tour != null ? x.Tour.TourOperatorId : (int?)null,
                    CreateAt = x.CreateAt,
                    PaymentAt = x.PaymentAt,
                    PaymentImg = x.PaymentImg
                }).ToList()
            };
        }

        public async Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request, int userId)
        {
            var tour = await _context.Tours.FirstOrDefaultAsync(t => t.TourId == request.TourId);
            if (tour == null)
                throw new Exception("Không tìm thấy tour. Vui lòng kiểm tra lại thông tin tour.");

            var departure = await _context.DepartureDates.FirstOrDefaultAsync(d => d.Id == request.DepartureDateId && d.TourId == request.TourId && d.IsActive);
            if (departure == null)
                throw new Exception("Không tìm thấy ngày khởi hành hoặc ngày khởi hành không hoạt động cho tour này. Vui lòng kiểm tra lại thông tin.");

            // Only allow booking if PaymentDeadline (DepartureDate - 21 days) is today or later
            var paymentDeadline = departure.DepartureDate1.AddDays(-21);
            if (paymentDeadline.Date < DateTime.UtcNow.Date)
                throw new Exception($"Không thể đặt tour vì đã qua hạn thanh toán. Hạn thanh toán là: {paymentDeadline:dd/MM/yyyy}, Ngày khởi hành: {departure.DepartureDate1:dd/MM/yyyy}. Vui lòng chọn ngày khởi hành khác.");

            int totalPeople = request.NumberOfAdults + request.NumberOfChildren + request.NumberOfInfants;
            int slotsBooked = tour.SlotsBooked ?? 0;
            int availableSlots = tour.MaxSlots - slotsBooked;
            if (totalPeople > availableSlots)
            {
                var message = availableSlots <= 0 
                    ? $"Rất tiếc! Tour này đã hết chỗ (MaxSlot: {tour.MaxSlots}, Đã đặt: {slotsBooked}). Vui lòng chọn tour khác hoặc liên hệ với chúng tôi để được hỗ trợ."
                    : $"Rất tiếc! Tour này chỉ còn {availableSlots} chỗ trống, nhưng bạn yêu cầu {totalPeople} chỗ. Vui lòng giảm số lượng người hoặc chọn tour khác.";
                throw new Exception(message);
            }

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
                IsActive = true,
                CreateAt = DateTime.UtcNow
            };
            Console.WriteLine($"DEBUG - Creating new booking with CreateAt: {booking.CreateAt}");
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
            
            Console.WriteLine($"DEBUG - Retrieved booking with CreateAt: {bookingWithNav.CreateAt}");

            // Send emails to customer and tour operator
            try
            {
                var customerEmail = bookingWithNav.User?.Email;
                var customerName = bookingWithNav.User?.UserName ?? "Khách hàng";
                if (!string.IsNullOrWhiteSpace(customerEmail))
                {
                    // Gửi email thông báo thanh toán cho khách hàng
                    var tourOperatorPhone = bookingWithNav.Tour?.TourOperator?.Hotline ?? "Không có thông tin";
                    await _emailService.SendBookingCreatedPaymentEmailAsync(
                        customerEmail, 
                        customerName, 
                        bookingWithNav.BookingId, 
                        bookingWithNav.TotalPrice ?? 0, 
                        paymentDeadline, 
                        tourOperatorPhone
                    );
                }

                var tourOperatorId = bookingWithNav.Tour != null ? bookingWithNav.Tour.TourOperatorId : (int?)null;
                var tourOperatorEntity = tourOperatorId.HasValue
                    ? await _context.TourOperators
                        .Include(to => to.User)
                        .FirstOrDefaultAsync(to => to.TourOperatorId == tourOperatorId.Value)
                    : null;
                var tourOpEmail = tourOperatorEntity?.User?.Email;
                var tourOpName = tourOperatorEntity?.CompanyName ?? "Tour Operator";
                if (!string.IsNullOrWhiteSpace(tourOpEmail))
                {
                    await _emailService.SendTourOperatorNotificationEmailAsync(
                        tourOpEmail,
                        tourOpName,
                        bookingWithNav.BookingId,
                        "Đặt tour mới",
                        $"Khách hàng {customerName} đã tạo đặt tour.");
                }
            }
            catch { }

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
                TourOperatorId = bookingWithNav.Tour != null ? bookingWithNav.Tour.TourOperatorId : (int?)null,
                CreateAt = bookingWithNav.CreateAt
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
            
            // Send emails to customer and tour operator about customer update
            try
            {
                var customerEmail = booking.User?.Email;
                var customerName = booking.User?.UserName ?? "Khách hàng";
                if (!string.IsNullOrWhiteSpace(customerEmail))
                {
                    await _emailService.SendBookingUpdateEmailAsync(customerEmail, customerName, booking.BookingId, "Khách hàng cập nhật đặt tour");
                }

                var tourOperatorId2 = booking.Tour != null ? booking.Tour.TourOperatorId : (int?)null;
                var tourOperatorEntity = tourOperatorId2.HasValue
                    ? await _context.TourOperators
                        .Include(to => to.User)
                        .FirstOrDefaultAsync(to => to.TourOperatorId == tourOperatorId2.Value)
                    : null;
                var tourOpEmail = tourOperatorEntity?.User?.Email;
                var tourOpName = tourOperatorEntity?.CompanyName ?? "Tour Operator";
                if (!string.IsNullOrWhiteSpace(tourOpEmail))
                {
                    await _emailService.SendTourOperatorNotificationEmailAsync(
                        tourOpEmail,
                        tourOpName,
                        booking.BookingId,
                        "Khách hàng cập nhật đặt tour",
                        $"Khách hàng {customerName} đã cập nhật thông tin đặt tour.");
                }
            }
            catch { }
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
                .Include(b => b.DepartureDate)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.IsActive);
            if (booking == null) return null;
            Console.WriteLine($"DEBUG - GetBookingById - Booking CreateAt: {booking.CreateAt}");
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
                TourOperatorId = booking.Tour != null ? booking.Tour.TourOperatorId : (int?)null,
                CreateAt = booking.CreateAt,
                PaymentAt = booking.PaymentAt,
                PaymentImg = booking.PaymentImg
            };
        }

        public async Task<BookingDetailResponse> GetBookingByIdDetailedAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Tour).ThenInclude(t => t.TourOperator)
                .Include(b => b.DepartureDate)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null) return null;
            return MapToBookingDetailResponse(booking);
        }

        public async Task<BookingResponse> UpdateBookingContractAsync(UpdateBookingRequest request)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Tour).ThenInclude(t => t.TourOperator)
                .Include(b => b.DepartureDate)
                .FirstOrDefaultAsync(x => x.BookingId == request.BookingId);
            if (booking == null) return null;
            booking.Contract = request.Contract;
            await _context.SaveChangesAsync();
            
            // Send emails to customer and tour operator about operator update
            try
            {
                var customerEmail = booking.User?.Email;
                var customerName = booking.User?.UserName ?? "Khách hàng";
                if (!string.IsNullOrWhiteSpace(customerEmail))
                {
                    await _emailService.SendBookingUpdateEmailAsync(customerEmail, customerName, booking.BookingId, "Nhà điều hành cập nhật hợp đồng");
                }

                var tourOperatorId3 = booking.Tour != null ? booking.Tour.TourOperatorId : (int?)null;
                var tourOperatorEntity = tourOperatorId3.HasValue
                    ? await _context.TourOperators
                        .Include(to => to.User)
                        .FirstOrDefaultAsync(to => to.TourOperatorId == tourOperatorId3.Value)
                    : null;
                var tourOpEmail = tourOperatorEntity?.User?.Email;
                var tourOpName = tourOperatorEntity?.CompanyName ?? "Tour Operator";
                if (!string.IsNullOrWhiteSpace(tourOpEmail))
                {
                    await _emailService.SendTourOperatorNotificationEmailAsync(
                        tourOpEmail,
                        tourOpName,
                        booking.BookingId,
                        "Cập nhật hợp đồng",
                        $"Bạn vừa cập nhật hợp đồng cho đặt tour #{booking.BookingId}.");
                }
            }
            catch { }
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
                    TourOperatorId = x.Tour != null ? x.Tour.TourOperatorId : (int?)null,
                    CreateAt = x.CreateAt,
                    PaymentAt = x.PaymentAt,
                    PaymentImg = x.PaymentImg
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
                .Where(x => x.Tour != null && x.Tour.TourOperatorId == tourOperator.TourOperatorId);

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
                    TourOperatorId = x.Tour != null ? x.Tour.TourOperatorId : (int?)null,
                    CreateAt = x.CreateAt,
                    PaymentAt = x.PaymentAt,
                    PaymentImg = x.PaymentImg
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
                .Where(x => x.UserId == userId);

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

        private BookingDetailResponse MapToBookingDetailResponse(Booking booking)
        {
            // Collect all guide notes from tour guide assignments for this tour and departure date
            var allGuideNotes = new List<GuideNotesInfo>();
            
            // Get guide notes from tour guide assignments for this tour and departure date
            var guideNotes = _context.TourGuideAssignments
                .Where(tga => tga.TourId == booking.TourId && 
                             tga.DepartureDateId == booking.DepartureDateId && 
                             tga.IsActive)
                .SelectMany(tga => tga.GuideNotes.Where(gn => gn.IsActive))
                .Select(gn => new GuideNotesInfo
                {
                    NoteId = gn.NoteId,
                    Title = gn.Title,
                    Content = gn.Content,
                    ExtraCost = gn.ExtraCost,
                    CreatedAt = gn.CreatedAt
                })
                .ToList();
            
            allGuideNotes.AddRange(guideNotes);

            return new BookingDetailResponse
            {
                BookingId = booking.BookingId,
                Tour = new TourInfo
                {
                    Title = booking.Tour?.Title ?? string.Empty,
                    MaxSlots = booking.Tour?.MaxSlots ?? 0,
                    Transportation = booking.Tour?.Transportation,
                    StartPoint = booking.Tour?.StartPoint,
                    DepartureDate = booking.DepartureDate?.DepartureDate1,
                    DurationInDays = booking.Tour?.DurationInDays
                },
                Booking = new BookingInfo
                {
                    BookingDate = booking.BookingDate,
                    Contract = booking.Contract,
                    NoteForTour = booking.NoteForTour,
                    CreateAt = booking.CreateAt,
                    PaymentAt = booking.PaymentAt,
                    PaymentImg = booking.PaymentImg
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
                },
                PaymentDeadline = booking.DepartureDate?.DepartureDate1.AddDays(-21),
                GuideNotes = allGuideNotes
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

            // Send emails to customer and tour operator
            try
            {
                var customerEmail = booking.User?.Email;
                var customerName = booking.User?.UserName ?? "Khách hàng";
                if (!string.IsNullOrWhiteSpace(customerEmail))
                {
                    await _emailService.SendBookingUpdateEmailAsync(
                        customerEmail,
                        customerName,
                        booking.BookingId,
                        $"Cập nhật trạng thái thanh toán: {request.PaymentStatus}");
                }

                var tourOperatorId4 = booking.Tour != null ? booking.Tour.TourOperatorId : (int?)null;
                var tourOperatorEntity = tourOperatorId4.HasValue
                    ? await _context.TourOperators
                        .Include(to => to.User)
                        .FirstOrDefaultAsync(to => to.TourOperatorId == tourOperatorId4.Value)
                    : null;
                var tourOpEmail = tourOperatorEntity?.User?.Email;
                var tourOpName = tourOperatorEntity?.CompanyName ?? "Tour Operator";
                if (!string.IsNullOrWhiteSpace(tourOpEmail))
                {
                    await _emailService.SendTourOperatorNotificationEmailAsync(
                        tourOpEmail,
                        tourOpName,
                        booking.BookingId,
                        "Cập nhật trạng thái thanh toán",
                        $"Trạng thái thanh toán của đặt tour #{booking.BookingId} đã được cập nhật: {request.PaymentStatus}.");
                }
            }
            catch { }

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

            // Send emails to customer and tour operator
            try
            {
                var customerEmail = booking.User?.Email;
                var customerName = booking.User?.UserName ?? "Khách hàng";
                if (!string.IsNullOrWhiteSpace(customerEmail))
                {
                    await _emailService.SendBookingUpdateEmailAsync(
                        customerEmail,
                        customerName,
                        booking.BookingId,
                        $"Cập nhật trạng thái đặt tour: {request.BookingStatus}");
                }

                var tourOperatorId5 = booking.Tour != null ? booking.Tour.TourOperatorId : (int?)null;
                var tourOperatorEntity = tourOperatorId5.HasValue
                    ? await _context.TourOperators
                        .Include(to => to.User)
                        .FirstOrDefaultAsync(to => to.TourOperatorId == tourOperatorId5.Value)
                    : null;
                var tourOpEmail = tourOperatorEntity?.User?.Email;
                var tourOpName = tourOperatorEntity?.CompanyName ?? "Tour Operator";
                if (!string.IsNullOrWhiteSpace(tourOpEmail))
                {
                    await _emailService.SendTourOperatorNotificationEmailAsync(
                        tourOpEmail,
                        tourOpName,
                        booking.BookingId,
                        "Cập nhật trạng thái đặt tour",
                        $"Trạng thái đặt tour #{booking.BookingId} đã được cập nhật: {request.BookingStatus}.");
                }
            }
            catch { }

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

        public async Task<BookingResponse> CancelBookingAsync(int bookingId, int userId)
        {
            // Validate booking exists and belongs to this user
            var booking = await _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.User)
                .Include(b => b.Tour).ThenInclude(t => t.TourOperator)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == userId && b.IsActive);

            if (booking == null)
                throw new Exception("Booking not found or you don't have permission to cancel it");

            // Check if booking can be cancelled
            if (booking.BookingStatus == StatusConstants.Booking.Cancelled)
                throw new Exception("Booking is already cancelled");

            if (booking.BookingStatus == StatusConstants.Booking.Completed)
                throw new Exception("Cannot cancel a completed booking");

            // Check if departure date is too close (e.g., within 24 hours)
            var departureDate = await _context.DepartureDates
                .FirstOrDefaultAsync(d => d.Id == booking.DepartureDateId);
            
            if (departureDate != null && departureDate.DepartureDate1 <= DateTime.Now.AddHours(24))
                throw new Exception("Cannot cancel booking within 24 hours of departure");

            // Update booking status to cancelled and hide it
            booking.BookingStatus = StatusConstants.Booking.Cancelled;
            booking.IsActive = false; // Hide the booking when cancelled

            // Release slots back to the tour
            var tour = booking.Tour;
            if (tour != null)
            {
                int totalPeople = (booking.NumberOfAdults ?? 0) + (booking.NumberOfChildren ?? 0) + (booking.NumberOfInfants ?? 0);
                tour.SlotsBooked = Math.Max(0, (tour.SlotsBooked ?? 0) - totalPeople);
            }

            await _context.SaveChangesAsync();

            // Send notification to tour operator
            if (tour != null)
            {
                // Get the user ID of the tour operator
                var tourOperatorUser = await _context.TourOperators
                    .Where(to => to.TourOperatorId == tour.TourOperatorId)
                    .Select(to => to.UserId)
                    .FirstOrDefaultAsync();
                
                if (tourOperatorUser.HasValue)
                {
                    await _notificationService.CreateNotificationAsync(
                        tourOperatorUser.Value,
                        "Booking Cancelled",
                        $"Booking #{booking.BookingId} has been cancelled by customer {booking.User?.UserName}",
                        "Booking",
                        booking.BookingId.ToString()
                    );
                    // Send email to tour operator
                    try
                    {
                        var tourOperatorEntity = await _context.TourOperators
                            .Include(to => to.User)
                            .FirstOrDefaultAsync(to => to.TourOperatorId == tour.TourOperatorId);
                        var tourOpEmail = tourOperatorEntity?.User?.Email;
                        var tourOpName = tourOperatorEntity?.CompanyName ?? "Tour Operator";
                        if (!string.IsNullOrWhiteSpace(tourOpEmail))
                        {
                            await _emailService.SendTourOperatorNotificationEmailAsync(
                                tourOpEmail,
                                tourOpName,
                                booking.BookingId,
                                "Huỷ đặt tour",
                                $"Khách hàng {booking.User?.UserName} đã huỷ đặt tour #{booking.BookingId}.");
                        }
                    }
                    catch { }
                }
            }

            // Send email to customer about cancellation
            try
            {
                var customerEmail = booking.User?.Email;
                var customerName = booking.User?.UserName ?? "Khách hàng";
                if (!string.IsNullOrWhiteSpace(customerEmail))
                {
                    await _emailService.SendBookingCancelledEmailAsync(customerEmail, customerName, booking.BookingId, "Huỷ bởi khách hàng");
                }
            }
            catch { }

            // Refund flows based on PaymentDeadline
            if (booking.PaymentStatus == PaymentStatus.Paid)
            {
                var departureDateEntity = await _context.DepartureDates.FirstOrDefaultAsync(d => d.Id == booking.DepartureDateId);
                if (departureDateEntity != null)
                {
                    var paymentDeadlineDate = departureDateEntity.DepartureDate1.AddDays(-21);
                    var refundWindowEnd = paymentDeadlineDate.AddDays(7);
                    var now = DateTime.UtcNow;

                    // Before departure only
                    if (now >= departureDateEntity.DepartureDate1)
                    {
                        throw new Exception("Không thể huỷ sau ngày khởi hành");
                    }

                    // Case 1: cancel from booking date up to 7 days after PaymentDeadline -> 100% refund
                    if ((booking.BookingDate == null || booking.BookingDate.Value <= now) && now <= refundWindowEnd)
                    {
                        await _notificationService.CreateNotificationAsync(
                            booking.UserId,
                            "Hoàn tiền 100% trong 48 giờ",
                            $"Yêu cầu huỷ đặt tour #{booking.BookingId} được ghi nhận. Chúng tôi sẽ hoàn lại 100% trong vòng 48 giờ.",
                            "Refund",
                            booking.BookingId.ToString()
                        );

                        // Email both customer and tour operator
                        var customerEmail = booking.User?.Email;
                        var customerName = booking.User?.UserName ?? "Khách hàng";
                        var tourOpEmail = booking.Tour?.TourOperator?.User?.Email;
                        var tourOpName = booking.Tour?.TourOperator?.CompanyName ?? "Tour Operator";
                        var refundAmount = booking.TotalPrice ?? 0;

                        if (!string.IsNullOrWhiteSpace(customerEmail))
                            await _emailService.SendRefundEmailAsync(customerEmail, customerName, booking.BookingId, refundAmount, "100%");
                        if (!string.IsNullOrWhiteSpace(tourOpEmail))
                            await _emailService.SendTourOperatorNotificationEmailAsync(tourOpEmail, tourOpName, booking.BookingId, "Yêu cầu hoàn 100%", "Khách hàng huỷ trong khung 100%, vui lòng xử lý hoàn tiền trong 48 giờ.");
                    }
                    // Case 2: cancel after 7 days after PaymentDeadline and before DepartureDate -> 70% refund
                    else if (now > refundWindowEnd && now < departureDateEntity.DepartureDate1)
                    {
                        await _notificationService.CreateNotificationAsync(
                            booking.UserId,
                            "Hoàn tiền 70% trong 48 giờ",
                            $"Yêu cầu huỷ đặt tour #{booking.BookingId} được ghi nhận. Chúng tôi sẽ hoàn lại 70% trong vòng 48 giờ.",
                            "Refund",
                            booking.BookingId.ToString()
                        );

                        // Email both customer and tour operator
                        var customerEmail = booking.User?.Email;
                        var customerName = booking.User?.UserName ?? "Khách hàng";
                        var tourOpEmail = booking.Tour?.TourOperator?.User?.Email;
                        var tourOpName = booking.Tour?.TourOperator?.CompanyName ?? "Tour Operator";
                        var refundAmount = (booking.TotalPrice ?? 0) * 0.7m;

                        if (!string.IsNullOrWhiteSpace(customerEmail))
                            await _emailService.SendRefundEmailAsync(customerEmail, customerName, booking.BookingId, refundAmount, "70%");
                        if (!string.IsNullOrWhiteSpace(tourOpEmail))
                            await _emailService.SendTourOperatorNotificationEmailAsync(tourOpEmail, tourOpName, booking.BookingId, "Yêu cầu hoàn 70%", "Khách hàng huỷ trong khung 70%, vui lòng xử lý hoàn tiền trong 48 giờ.");
                    }
                }
            }

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

        public async Task<BookingResponse> ToggleBookingVisibilityAsync(int bookingId, int tourOperatorId)
        {
            // Validate booking exists and belongs to this tour operator
            var booking = await _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                throw new Exception("Booking not found");

            if (booking.Tour?.TourOperatorId != tourOperatorId)
                throw new Exception("You don't have permission to update this booking");

            // Toggle IsActive status
            booking.IsActive = !booking.IsActive;

            await _context.SaveChangesAsync();

            // Send notification to customer about visibility change
            await _notificationService.CreateNotificationAsync(
                booking.UserId,
                "Booking Visibility Updated",
                $"Your booking #{booking.BookingId} visibility has been {(booking.IsActive ? "enabled" : "disabled")} by the tour operator",
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

        public async Task<BookingResponse> UpdateBookingPaymentAsync(UpdateBookingPaymentRequest request, int userId)
        {
            // First, check if the user is customer by getting their role
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                throw new Exception("User not found");

            if (user.Role.RoleName != "Customer")
                throw new Exception("Only customers can update booking payment");

            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Tour).ThenInclude(t => t.TourOperator)
                .FirstOrDefaultAsync(b => b.BookingId == request.BookingId && b.IsActive);

            if (booking == null)
                throw new Exception("Booking not found or inactive");

            if (booking.UserId != userId)
                throw new Exception("You don't have permission to update this booking's payment");

            if (booking.BookingStatus == StatusConstants.Booking.Cancelled)
                throw new Exception("Cannot update payment for cancelled booking");

            // Handle payment image upload
            string imageUrl = null;
            if (request.PaymentImage != null)
            {
                // Generate unique filename
                var extension = Path.GetExtension(request.PaymentImage.FileName);
                var fileName = $"payment_{booking.BookingId}_{DateTime.UtcNow.Ticks}{extension}";
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "payments");

                // Ensure directory exists
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Save file
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.PaymentImage.CopyToAsync(fileStream);
                }

                // Set relative URL path
                imageUrl = $"/uploads/payments/{fileName}";
            }

            // Update payment information
            booking.PaymentAt = DateTime.UtcNow;
            if (imageUrl != null)
            {
                booking.PaymentImg = imageUrl;
            }
            booking.PaymentStatus = StatusConstants.Payment.Pending;

            await _context.SaveChangesAsync();

            // Send notification to tour operator about new payment proof
            var tourOperatorId = booking.Tour?.TourOperatorId;
            if (tourOperatorId.HasValue)
            {
                var tourOperator = await _context.TourOperators
                    .Include(to => to.User)
                    .FirstOrDefaultAsync(to => to.TourOperatorId == tourOperatorId.Value);
                
                if (tourOperator != null)
                {
                    var tourOperatorUserId = tourOperator.User?.UserId ?? 0;
                    if (tourOperatorUserId != 0)
                    {
                        await _notificationService.CreateNotificationAsync(
                            tourOperatorUserId,
                            "New Payment Proof Submitted",
                            $"Customer has submitted payment proof for booking #{booking.BookingId}. Please review and update payment status.",
                            "Payment",
                            booking.BookingId.ToString()
                        );

                        // Send email to tour operator
                        try
                        {
                            var tourOperatorEntity = await _context.TourOperators
                                .Include(to => to.User)
                                .FirstOrDefaultAsync(to => to.TourOperatorId == tourOperatorId.Value);
                            var tourOpEmail = tourOperatorEntity?.User?.Email;
                            var tourOpName = tourOperatorEntity?.CompanyName ?? "Tour Operator";
                            if (!string.IsNullOrWhiteSpace(tourOpEmail))
                            {
                                await _emailService.SendTourOperatorNotificationEmailAsync(
                                    tourOpEmail,
                                    tourOpName,
                                    booking.BookingId,
                                    "Bằng chứng thanh toán mới",
                                    $"Khách hàng {booking.User?.UserName} đã gửi bằng chứng thanh toán cho đặt tour #{booking.BookingId}. Vui lòng xem xét và cập nhật trạng thái thanh toán.");
                            }
                        }
                        catch { }
                    }
                }
            }

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
                TourOperatorId = booking.Tour?.TourOperatorId,
                CreateAt = booking.CreateAt,
                PaymentAt = booking.PaymentAt,
                PaymentImg = booking.PaymentImg
            };
        }
    }
} 