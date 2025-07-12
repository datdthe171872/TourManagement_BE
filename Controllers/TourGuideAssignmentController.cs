using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;

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

        [HttpGet("test-auth")]
        [Authorize]
        public IActionResult TestAuth()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);
            
            return Ok(new { 
                message = "Authentication successful", 
                userId, 
                email, 
                role,
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet]
        [Authorize(Roles = "Tour Operator")]
        [ProducesResponseType(typeof(TourGuideAssignmentListResponse), 200)]
        public async Task<IActionResult> GetAssignments([FromQuery] TourGuideAssignmentSearchRequest request)
        {
            var query = _context.TourGuideAssignments
                .Include(a => a.TourGuide)
                .ThenInclude(tg => tg.User)
                .Include(a => a.Booking)
                .ThenInclude(b => b.User)
                .Include(a => a.Booking)
                .ThenInclude(b => b.Tour)
                .AsQueryable();

            // Apply filters
            if (request.TourId.HasValue)
                query = query.Where(a => a.TourId == request.TourId.Value);

            if (request.TourGuideId.HasValue)
                query = query.Where(a => a.TourGuideId == request.TourGuideId.Value);

            if (request.BookingId.HasValue)
                query = query.Where(a => a.BookingId == request.BookingId.Value);

            if (request.IsLeadGuide.HasValue)
                query = query.Where(a => a.IsLeadGuide == request.IsLeadGuide.Value);

            if (request.IsActive.HasValue)
                query = query.Where(a => a.IsActive == request.IsActive.Value);

            if (request.AssignedDateFrom.HasValue)
                query = query.Where(a => a.AssignedDate >= DateOnly.FromDateTime(request.AssignedDateFrom.Value));

            if (request.AssignedDateTo.HasValue)
                query = query.Where(a => a.AssignedDate <= DateOnly.FromDateTime(request.AssignedDateTo.Value));

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var assignments = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(a => new TourGuideAssignmentResponse
                {
                    Id = a.Id,
                    TourId = a.TourId,
                    BookingId = a.BookingId,
                    TourGuideId = a.TourGuideId,
                    AssignedDate = a.AssignedDate,
                    NoteId = a.NoteId,
                    IsLeadGuide = a.IsLeadGuide,
                    IsActive = a.IsActive,
                    TourName = a.Booking.Tour.TourType,
                    GuideName = a.TourGuide.User.UserName,
                    CustomerName = a.Booking.User.UserName
                })
                .ToListAsync();

            var response = new TourGuideAssignmentListResponse
            {
                Assignments = assignments,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            return Ok(response);
        }
    }
} 