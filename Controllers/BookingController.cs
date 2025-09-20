using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Repository.Interface;
using TourManagement_BE.Service;
using TourManagement_BE.Helper.Constant;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace TourManagement_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IUserRepository _userRepository;
        private readonly MyDBContext _dbContext;

        public BookingController(IBookingService bookingService, IUserRepository userRepository, MyDBContext dbContext)
        {
            _bookingService = bookingService;
            _userRepository = userRepository;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings([FromQuery] BookingSearchRequest request)
        {
            var result = await _bookingService.GetBookingsDetailedAsync(request);
            return Ok(result);
        }

        [HttpGet("{bookingId}/detailed")]
        public async Task<IActionResult> GetBookingByIdDetailed(int bookingId)
        {
            var result = await _bookingService.GetBookingByIdDetailedAsync(bookingId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            // Validate số lượng người > 0
            if (request.NumberOfAdults < 0 || request.NumberOfChildren < 0 || request.NumberOfInfants < 0)
                return BadRequest("Số lượng người không hợp lệ");
            if (request.NumberOfAdults + request.NumberOfChildren + request.NumberOfInfants <= 0)
                return BadRequest("Tổng số người phải lớn hơn 0");
            // Lấy UserId từ token
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Không xác định được người dùng");
            int userId = int.Parse(userIdClaim.Value);
            // Chỉ cho phép đặt tour khi PaymentDeadline (DepartureDate - 21 ngày) từ hôm nay trở về sau
            var departure = await _dbContext.DepartureDates
                .FirstOrDefaultAsync(d => d.Id == request.DepartureDateId && d.TourId == request.TourId && d.IsActive);
            if (departure == null)
                return BadRequest("Ngày khởi hành không hợp lệ hoặc không hoạt động cho tour này");

            var paymentDeadline = departure.DepartureDate1.AddDays(-21);
            if (paymentDeadline.Date < DateTime.UtcNow.Date)
                return BadRequest("Không thể đặt tour vì đã qua hạn thanh toán (PaymentDeadline)");
            var result = await _bookingService.CreateBookingAsync(request, userId);
            return Ok(result);
        }

        [HttpPut("customer-update")]
        [Authorize(Roles = Roles.Customer)]
        public async Task<IActionResult> UpdateBookingCustomer([FromBody] UpdateBookingCustomerRequest request)
        {
            var booking = await _bookingService.GetBookingByIdAsync(request.BookingId);
            if (booking == null) return NotFound();
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Không xác định được người dùng");
            int userId = int.Parse(userIdClaim.Value);
            if (booking.UserId != userId)
                return Forbid();
            var updateRequest = new UpdateBookingRequest
            {
                BookingId = request.BookingId,
                DepartureDateId = request.DepartureDateId,
                NumberOfAdults = request.NumberOfAdults,
                NumberOfChildren = request.NumberOfChildren,
                NumberOfInfants = request.NumberOfInfants,
                NoteForTour = request.NoteForTour
            };
            var result = await _bookingService.UpdateBookingAsync(updateRequest);
            return Ok(result);
        }

        [HttpPut("operator-update")]
        [Authorize(Roles = Roles.TourOperator)]
        public async Task<IActionResult> UpdateBookingOperator([FromBody] UpdateBookingOperatorRequest request)
        {
            var booking = await _bookingService.GetBookingByIdAsync(request.BookingId);
            if (booking == null) return NotFound();
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Không xác định được người dùng");
            int userId = int.Parse(userIdClaim.Value);
            if (booking.TourOperatorId != userId)
                return Forbid();
            var updateRequest = new UpdateBookingRequest
            {
                BookingId = request.BookingId,
                Contract = request.Contract
            };
            var result = await _bookingService.UpdateBookingContractAsync(updateRequest);
            return Ok(result);
        }

        [HttpPatch("delete/{bookingId}")]
        public async Task<IActionResult> SoftDeleteBooking(int bookingId, [FromQuery] int userId)
        {
            var result = await _bookingService.SoftDeleteBookingAsync(bookingId, userId);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpPut("cancel/{bookingId}")]
        [Authorize(Roles = Roles.Customer)]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized("Không xác định được người dùng");
                
                int userId = int.Parse(userIdClaim.Value);
                
                var result = await _bookingService.CancelBookingAsync(bookingId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // API 1: Get bookings for Customer (Customer role only)
        [HttpGet("customer")]
        [Authorize(Roles = Roles.Customer)]
        public async Task<IActionResult> GetCustomerBookings([FromQuery] BookingSearchCustomerRequest request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Không xác định được người dùng");
            int userId = int.Parse(userIdClaim.Value);
            var searchRequest = new BookingSearchRequest
            {
                TourName = request.TourName
            };
            var result = await _bookingService.GetCustomerBookingsDetailedAsync(searchRequest, userId);
            return Ok(result);
        }

        // API 2: Get bookings for Tour Operator (Tour Operator role only)
        [HttpGet("tour-operator")]
        [Authorize(Roles = Roles.TourOperator)]
        public async Task<IActionResult> GetTourOperatorBookings([FromQuery] BookingSearchTourOperatorRequest request)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Không xác định được người dùng");
            int userId = int.Parse(userIdClaim.Value);
            
            var searchRequest = new BookingSearchRequest
            {
                TourName = request.TourName,
                UserName = request.UserName
            };
            var result = await _bookingService.GetTourOperatorBookingsDetailedAsync(searchRequest, userId);
            return Ok(result);
        }

        // API 3: Get all bookings for Admin (Admin role only)
        [HttpGet("admin")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetAllBookingsForAdmin([FromQuery] BookingSearchAdminRequest request)
        {
            var searchRequest = new BookingSearchRequest
            {
                TourName = request.TourName,
                UserName = request.UserName
            };
            var result = await _bookingService.GetAllBookingsForAdminDetailedAsync(searchRequest);
            return Ok(result);
        }

        // API 4: Update Payment Status (Tour Operator role only)
        [HttpPut("update-payment-status")]
        [Authorize(Roles = Roles.TourOperator)]
        public async Task<IActionResult> UpdatePaymentStatus([FromBody] UpdatePaymentStatusRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized("Không xác định được người dùng");
                
                int userId = int.Parse(userIdClaim.Value);
                
                // Get tour operator ID from user ID
                var tourOperator = await _dbContext.TourOperators
                    .FirstOrDefaultAsync(to => to.UserId == userId);
                
                if (tourOperator == null)
                    return Forbid("Bạn không phải là Tour Operator");
                
                var result = await _bookingService.UpdatePaymentStatusAsync(request, tourOperator.TourOperatorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // API 5: Update Booking Status (Tour Operator role only)
        [HttpPut("update-booking-status")]
        [Authorize(Roles = Roles.TourOperator)]
        public async Task<IActionResult> UpdateBookingStatus([FromBody] UpdateBookingStatusRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized("Không xác định được người dùng");
                
                int userId = int.Parse(userIdClaim.Value);
                
                // Get tour operator ID from user ID
                var tourOperator = await _dbContext.TourOperators
                    .FirstOrDefaultAsync(to => to.UserId == userId);
                
                if (tourOperator == null)
                    return Forbid("Bạn không phải là Tour Operator");
                
                var result = await _bookingService.UpdateBookingStatusAsync(request, tourOperator.TourOperatorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                                 return BadRequest(ex.Message);
             }
         }

        // API 6: Toggle Booking Visibility (Tour Operator role only)
        [HttpPut("toggle-visibility/{bookingId}")]
        [Authorize(Roles = Roles.TourOperator)]
        public async Task<IActionResult> ToggleBookingVisibility(int bookingId)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized("Không xác định được người dùng");
                
                int userId = int.Parse(userIdClaim.Value);
                
                // Get tour operator ID from user ID
                var tourOperator = await _dbContext.TourOperators
                    .FirstOrDefaultAsync(to => to.UserId == userId);
                
                if (tourOperator == null)
                    return Forbid("Bạn không phải là Tour Operator");
                
                var result = await _bookingService.ToggleBookingVisibilityAsync(bookingId, tourOperator.TourOperatorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // API 7: Update Booking Payment (Customer role only)
        [HttpPut("update-paymentdate-img")]
        [Authorize(Roles = Roles.Customer)]
        public async Task<IActionResult> UpdateBookingPayment([FromForm] UpdateBookingPaymentRequest request)
        {
            try
            {
                if (request.PaymentImage == null || request.PaymentImage.Length == 0)
                    return BadRequest("Vui lòng chọn ảnh thanh toán");

                // Kiểm tra định dạng file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(request.PaymentImage.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("Chỉ chấp nhận file ảnh có định dạng .jpg, .jpeg hoặc .png");

                // Kiểm tra kích thước file (ví dụ: max 5MB)
                if (request.PaymentImage.Length > 5 * 1024 * 1024)
                    return BadRequest("Kích thước file không được vượt quá 5MB");

                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized("Không xác định được người dùng");
                
                int userId = int.Parse(userIdClaim.Value);
                
                var result = await _bookingService.UpdateBookingPaymentAsync(request, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
} 