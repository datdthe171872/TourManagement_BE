using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.TourItineraryRequest;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Repository.Interface;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourItineraryController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly ISlotCheckService _slotCheckService;
        public TourItineraryController(MyDBContext context, IMapper mapper, Cloudinary cloudinary, ISlotCheckService slotCheckService)
        {
            this.context = context;
            _mapper = mapper;
            this._cloudinary = cloudinary;
            _slotCheckService = slotCheckService;
        }

        [HttpPost("CreateTourItinerary")]
        public async Task<IActionResult> CreateTourItinerary([FromForm] TourItineraryCreateRequest request)
        {
            var tour = await context.Tours
                .Include(t => t.TourItineraries)
                .FirstOrDefaultAsync(t => t.TourId == request.TourId);

            if (tour == null)
                return NotFound("Tour not found.");

            // Kiểm tra giới hạn theo gói dịch vụ
            var slotInfo = await _slotCheckService.CheckRemainingSlotsAsync(tour.TourOperatorId);
            if (slotInfo == null)
                return BadRequest("No remaining service package. Please purchase a package to create itinerary.");

            int maxTourItineraries = slotInfo.NumberOfTourAttribute == 0 ? int.MaxValue : slotInfo.NumberOfTourAttribute;

            int currentActiveItineraries = tour.TourItineraries.Count(i => i.IsActive);
            if (currentActiveItineraries >= maxTourItineraries)
            {
                return BadRequest($"You have reached the maximum number of tour itineraries ({maxTourItineraries}) allowed by your current package.");
            }

            // Kiểm tra số ngày tối đa theo DurationInDays
            if (!int.TryParse(tour.DurationInDays, out var maxDays))
                return BadRequest("DurationInDays của tour không hợp lệ.");

            if (tour.TourItineraries.Count >= maxDays)
                return BadRequest($"Tour đã đủ {maxDays} ngày lịch trình. Không thể thêm mới.");

            int nextDayNumber = tour.TourItineraries.Any()
                ? tour.TourItineraries.Max(i => i.DayNumber) + 1
                : 1;

            var iti = new TourItinerary
            {
                DayNumber = nextDayNumber,
                Title = request.Title,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                IsActive = true
            };

            tour.TourItineraries.Add(iti);
            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "TourItinerary created successfully.",
                itineraryId = iti.ItineraryId,
                dayNumber = iti.DayNumber
            });
        }




        [HttpDelete("SoftDeleteTourItinerary/{id}")]
        public async Task<IActionResult> SoftDeleteTourItinerary(int id)
        {
            var iti = await context.TourItineraries.FindAsync(id);
            if (iti == null)
                return NotFound("TourItinerary not found.");

            iti.IsActive = false;
            await context.SaveChangesAsync();

            return Ok(new { message = "TourItinerary deactivated successfully." });
        }

        [HttpPatch("ToggleTourItinerary/{id}")]
        public async Task<IActionResult> ToggleTourItinerary(int id)
        {
            var iti = await context.TourItineraries.FindAsync(id);
            if (iti == null)
                return NotFound("TourItinerary not found.");

            iti.IsActive = !iti.IsActive;

            await context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Tour Itinerary has been {(iti.IsActive ? "activated" : "deactivated")}",
            });
        }
    }
}
