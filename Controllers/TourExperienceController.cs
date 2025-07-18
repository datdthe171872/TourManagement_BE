using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data;
using TourManagement_BE.Data.DTO.Request.TourExperienceRequest;
using TourManagement_BE.Models;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourExperienceController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;

        public TourExperienceController(MyDBContext context, IMapper mapper, Cloudinary cloudinary)
        {
            this.context = context;
            _mapper = mapper;
            this._cloudinary = cloudinary;
        }

        [HttpPost("CreateTourExperience")]
        public async Task<IActionResult> CreateTourExperience([FromBody] CreateTourExperience request)
        {
            var tour = await context.Tours.FindAsync(request.TourId);
            if (tour == null)
                return NotFound("Tour not found.");

            var exp = new TourExperience
            {
                Content = request.Content,
                IsActive = true
            };
            tour.TourExperiences.Add(exp);
            await context.SaveChangesAsync();

            return Ok(new { message = "TourExperience created successfully.", id = exp.Id });
        }

        [HttpDelete("SoftDeleteTourExperience/{id}")]
        public async Task<IActionResult> SoftDeleteTourExperience(int id)
        {
            var exp = await context.TourExperiences.FindAsync(id);
            if (exp == null)
                return NotFound("TourExperience not found.");

            exp.IsActive = false;
            await context.SaveChangesAsync();

            return Ok(new { message = "TourExperience deactivated successfully." });
        }

    }
}
