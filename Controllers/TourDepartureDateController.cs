using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.DepartureDatesRequest;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Helper.Constant;
using TourManagement_BE.Repository.Interface;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.TourOperator)]
    public class TourDepartureDateController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly ISlotCheckService _slotCheckService;

        public TourDepartureDateController(MyDBContext context, IMapper mapper, Cloudinary cloudinary, ISlotCheckService slotCheckService)
        {
            this.context = context;
            _mapper = mapper;
            this._cloudinary = cloudinary;
            _slotCheckService = slotCheckService;
        }

        [HttpPost("CreateDepartureDate")]
        public async Task<IActionResult> CreateDepartureDate([FromBody] CreateDepartureDate request)
        {
            var tour = await context.Tours
                .Include(t => t.DepartureDates)
                .FirstOrDefaultAsync(t => t.TourId == request.TourId);

            if (tour == null)
                return NotFound("Tour not found.");

            // Lấy thông tin gói dịch vụ đang sử dụng
            var slotInfo = await _slotCheckService.CheckRemainingSlotsAsync(tour.TourOperatorId);
            if (slotInfo == null)
                return BadRequest("No remaining time to create departure date. Please purchase a service package.");

            /*int maxDepartureDates = slotInfo.NumberOfTourAttribute == 0 ? int.MaxValue : slotInfo.NumberOfTourAttribute;*/

            int currentActiveDepartureDates = tour.DepartureDates.Count(d => d.IsActive);
            /*if (currentActiveDepartureDates >= maxDepartureDates)
            {
                return BadRequest($"You have reached the maximum number of departure dates ({maxDepartureDates}) for your current package.");
            }*/

            var today = DateTime.UtcNow.AddHours(7).Date;

            if (request.DepartureDate1.Date <= today)
                return BadRequest("Departure date must be greater than today.");

            if (tour.DepartureDates.Any(d => d.IsActive && d.DepartureDate1.Date == request.DepartureDate1.Date))
            {
                return BadRequest("Departure date already exists for this tour.");
            }

            var dep = new DepartureDate
            {
                DepartureDate1 = request.DepartureDate1,
                IsCancelDate = false,
                IsActive = true
            };

            tour.DepartureDates.Add(dep);
            await context.SaveChangesAsync();

            return Ok(new { message = "DepartureDate created successfully.", id = dep.Id });
        }


        [HttpDelete("SoftDeleteDepartureDate/{id}")]
        public async Task<IActionResult> SoftDeleteDepartureDate(int id)
        {
            var dep = await context.DepartureDates
                .Include(d => d.Bookings)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dep == null)
                return NotFound("DepartureDate not found.");

            var today = DateTime.UtcNow.AddHours(7).Date;

            // 1) Không cho xoá nếu ngày đã qua
            if (dep.DepartureDate1.Date < today)
                return BadRequest("Cannot delete a departure date in the past.");

            // 2) Không cho xoá nếu đã có booking còn hiệu lực
            var hasActiveBooking = dep.Bookings.Any(b => b.BookingStatus != "Cancelled");
            if (hasActiveBooking)
                return BadRequest("This departure date already has bookings. You cannot delete it.");

            dep.IsActive = false;
            await context.SaveChangesAsync();

            return Ok(new { message = "DepartureDate deactivated successfully." });
        }

        [HttpPatch("ToggleDepartureDateStatus/{id}")]
        public async Task<IActionResult> ToggleDepartureDateStatus(int id)
        {
            var dep = await context.DepartureDates
                .Include(d => d.Bookings)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dep == null)
                return NotFound("DepartureDate not found.");

            if (dep.IsActive)
            {
                var today = DateTime.UtcNow.AddHours(7).Date;

                if (dep.DepartureDate1.Date < today)
                    return BadRequest("Cannot deactivate a departure date in the past.");

                // 2) Không cho deactivate nếu đã có booking còn hiệu lực
                var hasActiveBooking = dep.Bookings.Any(b => b.BookingStatus != "Cancelled");
                if (hasActiveBooking)
                    return BadRequest("Cannot deactivate - this departure date has active bookings.");
            }

            dep.IsActive = !dep.IsActive;

            await context.SaveChangesAsync();

            return Ok(new
            {
                message = $"DepartureDate has been {(dep.IsActive ? "activated" : "deactivated")}",
                departureDateId = id,
                newStatus = dep.IsActive,
                departureDate = dep.DepartureDate1.ToString("yyyy-MM-dd")
            });
        }
    }
}
