using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request;
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
            var result = await _bookingService.GetBookingsAsync(request);
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
                Keyword = request.Keyword
            };
            var result = await _bookingService.GetCustomerBookingsAsync(searchRequest, userId);
            return Ok(result);
        }

        // API 2: Get bookings for Tour Operator (Tour Operator role only)
        [HttpGet("tour-operator")]
        [Authorize(Roles = Roles.TourOperator)]
        public async Task<IActionResult> GetTourOperatorBookings([FromQuery] BookingSearchTourOperatorRequest request)
        {
            var searchRequest = new BookingSearchRequest
            {
                Keyword = request.Keyword
            };
            var result = await _bookingService.GetTourOperatorBookingsAsync(searchRequest);
            return Ok(result);
        }

        // API 3: Get all bookings for Admin (Admin role only)
        [HttpGet("admin")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetAllBookingsForAdmin([FromQuery] BookingSearchAdminRequest request)
        {
            var searchRequest = new BookingSearchRequest
            {
                Keyword = request.Keyword
            };
            var result = await _bookingService.GetAllBookingsForAdminAsync(searchRequest);
            return Ok(result);
        }
    }
} 