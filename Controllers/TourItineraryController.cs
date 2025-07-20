using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data;
using TourManagement_BE.Data.DTO.Request.TourItineraryRequest;
using TourManagement_BE.Models;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourItineraryController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;

        public TourItineraryController(MyDBContext context, IMapper mapper, Cloudinary cloudinary)
        {
            this.context = context;
            _mapper = mapper;
            this._cloudinary = cloudinary;
        }

        /*[HttpPost("CreateTourItinerary")]
        public async Task<IActionResult> CreateTourItinerary([FromForm] TourItineraryCreateRequest request)
        {
            var tour = await context.Tours.FindAsync(request.TourId);
            if (tour == null)
                return NotFound("Tour not found.");

            var iti = new TourItinerary
            {
                DayNumber = request.DayNumber,
                Title = request.Title,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                IsActive = true
            };

            tour.TourItineraries.Add(iti);
            await context.SaveChangesAsync();

            return Ok(new { message = "TourItinerary created successfully.", id = iti.ItineraryId });
        }*/

        [HttpPost("CreateTourItinerary")]
        public async Task<IActionResult> CreateTourItinerary([FromForm] TourItineraryCreateRequest request)
        {
            var tour = await context.Tours
                .Include(t => t.TourItineraries)
                .FirstOrDefaultAsync(t => t.TourId == request.TourId);

            if (tour == null)
                return NotFound("Tour not found.");

            int nextDayNumber = 1;
            if (tour.TourItineraries.Any())
                nextDayNumber = tour.TourItineraries.Max(i => i.DayNumber) + 1;

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
    }
}
