using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.TourExperienceRequest;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Helper.Constant;
using TourManagement_BE.Repository.Interface;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.TourOperator)]
    public class TourExperienceController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly ISlotCheckService _slotCheckService;

        public TourExperienceController(MyDBContext context, IMapper mapper, Cloudinary cloudinary, ISlotCheckService slotCheckService)
        {
            this.context = context;
            _mapper = mapper;
            this._cloudinary = cloudinary;
            _slotCheckService = slotCheckService;
        }

        [HttpPost("CreateTourExperience")]
        public async Task<IActionResult> CreateTourExperience([FromBody] CreateTourExperience request)
        {
            var tour = await context.Tours
                .Include(t => t.TourExperiences)
                .FirstOrDefaultAsync(t => t.TourId == request.TourId);

            if (tour == null)
                return NotFound("Tour not found.");

            var slotInfo = await _slotCheckService.CheckRemainingSlotsAsync(tour.TourOperatorId);
            if (slotInfo == null)
                return BadRequest("No remaining time to create tour experience. Please purchase a service package.");

            //int maxTourExperiences = slotInfo.NumberOfTourAttribute == 0 ? int.MaxValue : slotInfo.NumberOfTourAttribute;

            int currentActiveExperiences = tour.TourExperiences.Count(e => e.IsActive);
            /*if (currentActiveExperiences >= maxTourExperiences)
            {
                return BadRequest($"You have reached the maximum number of tour experiences ({maxTourExperiences}) for your current package.");
            }*/

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

        [HttpPatch("ToggleTourExperience/{id}")]
        public async Task<IActionResult> ToggleTourExperience(int id)
        {
            var exp = await context.TourExperiences.FindAsync(id);
            if (exp == null)
                return NotFound("TourExperience not found.");

            exp.IsActive = !exp.IsActive;

            await context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Tour Experience has been {(exp.IsActive ? "activated" : "deactivated")}",
            });
        }

    }
}
