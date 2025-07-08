using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Tour Operator")]
    public class TourGuideAssignmentController : ControllerBase
    {
        private readonly MyDBContext _context;
        public TourGuideAssignmentController(MyDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssignment([FromBody] CreateTourGuideAssignmentRequest request)
        {
            var assignment = new TourGuideAssignment
            {
                TourId = request.TourId,
                BookingId = request.BookingId,
                TourGuideId = request.TourGuideId,
                AssignedDate = request.AssignedDate != null ? DateOnly.FromDateTime(request.AssignedDate.Value) : null,
                IsLeadGuide = request.IsLeadGuide,
                IsActive = true
            };
            _context.TourGuideAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            return Ok("Assignment created successfully");
        }

        [HttpGet("my-assignments")]
        [Authorize(Roles = "Tour Guide")]
        [ProducesResponseType(typeof(List<object>), 200)]
        public async Task<IActionResult> GetMyAssignments()
        {
            var userId = int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier));
            // Lấy TourGuideId từ UserId
            var guide = await _context.TourGuides.FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            if (guide == null) return NotFound("Guide not found");
            var assignments = await _context.TourGuideAssignments
                .Where(a => a.TourGuideId == guide.TourGuideId && a.IsActive)
                .Select(a => new {
                    a.Id,
                    a.TourId,
                    a.BookingId,
                    a.AssignedDate,
                    a.IsLeadGuide
                })
                .ToListAsync();
            return Ok(assignments);
        }
    }
} 