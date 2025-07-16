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
            var result = await _bookingService.CreateBookingAsync(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBooking([FromBody] UpdateBookingRequest request)
        {
            var result = await _bookingService.UpdateBookingAsync(request);
            if (result == null) return NotFound();
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
        public async Task<IActionResult> GetCustomerBookings([FromQuery] BookingSearchRequest request)
        {
            var result = await _bookingService.GetCustomerBookingsAsync(request);
            return Ok(result);
        }

        // API 2: Get bookings for Tour Operator (Tour Operator role only)
        [HttpGet("tour-operator")]
        [Authorize(Roles = Roles.TourOperator)]
        public async Task<IActionResult> GetTourOperatorBookings([FromQuery] BookingSearchRequest request)
        {
            var result = await _bookingService.GetTourOperatorBookingsAsync(request);
            return Ok(result);
        }

        // API 3: Get all bookings for Admin (Admin role only)
        [HttpGet("admin")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetAllBookingsForAdmin([FromQuery] BookingSearchRequest request)
        {
            var result = await _bookingService.GetAllBookingsForAdminAsync(request);
            return Ok(result);
        }
    }
} 