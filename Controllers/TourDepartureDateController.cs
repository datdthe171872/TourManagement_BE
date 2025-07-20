using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.DepartureDatesRequest;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourDepartureDateController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;

        public TourDepartureDateController(MyDBContext context, IMapper mapper, Cloudinary cloudinary)
        {
            this.context = context;
            _mapper = mapper;
            this._cloudinary = cloudinary;
        }

        [HttpPost("CreateDepartureDate")]
        public async Task<IActionResult> CreateDepartureDate([FromBody] CreateDepartureDate request)
        {
            var tour = await context.Tours.FindAsync(request.TourId);
            if (tour == null)
                return NotFound("Tour not found.");

            if (request.DepartureDate1.Date < DateTime.UtcNow.AddHours(7).Date)
                return BadRequest("Departure date cannot be in the past.");

            var dep = new DepartureDate
            {
                DepartureDate1 = request.DepartureDate1,
                IsActive = true
            };
            tour.DepartureDates.Add(dep);
            await context.SaveChangesAsync();

            return Ok(new { message = "DepartureDate created successfully.", id = dep.Id });
        }

        [HttpDelete("SoftDeleteDepartureDate/{id}")]
        public async Task<IActionResult> SoftDeleteDepartureDate(int id)
        {
            var dep = await context.DepartureDates.FindAsync(id);
            if (dep == null)
                return NotFound("DepartureDate not found.");

            dep.IsActive = false;
            await context.SaveChangesAsync();

            return Ok(new { message = "DepartureDate deactivated successfully." });
        }
    }
}
