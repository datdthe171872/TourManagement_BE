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
using System.Collections.Generic;

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
            // Kiểm tra xem tour guide đã được assign cho departure date này chưa
            var existingAssignment = await _context.TourGuideAssignments
                .FirstOrDefaultAsync(a => a.TourId == request.TourId && 
                                        a.DepartureDateId == request.DepartureDateId && 
                                        a.TourGuideId == request.TourGuideId && 
                                        a.IsActive);
            
            if (existingAssignment != null)
            {
                return BadRequest("Tour guide đã được assign cho departure date này");
            }

            var assignment = new TourGuideAssignment
            {
                TourId = request.TourId,
                DepartureDateId = request.DepartureDateId,
                TourGuideId = request.TourGuideId,
                AssignedDate = request.AssignedDate != null ? DateOnly.FromDateTime(request.AssignedDate.Value) : null,
                IsLeadGuide = request.IsLeadGuide,
                IsActive = true
            };
            _context.TourGuideAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            return Ok("Assignment created successfully");
        }

        [HttpPost("multiple")]
        public async Task<IActionResult> CreateMultipleAssignments([FromBody] CreateMultipleTourGuideAssignmentRequest request)
        {
            if (request.TourGuides == null || !request.TourGuides.Any())
            {
                return BadRequest("Danh sách tour guide không được để trống");
            }

            var assignments = new List<TourGuideAssignment>();
            var existingAssignments = await _context.TourGuideAssignments
                .Where(a => a.TourId == request.TourId && 
                           a.DepartureDateId == request.DepartureDateId && 
                           a.IsActive)
                .ToListAsync();

            foreach (var tourGuide in request.TourGuides)
            {
                // Kiểm tra xem tour guide đã được assign chưa
                var existingAssignment = existingAssignments
                    .FirstOrDefault(a => a.TourGuideId == tourGuide.TourGuideId);
                
                if (existingAssignment != null)
                {
                    continue; // Bỏ qua nếu đã được assign
                }

                var assignment = new TourGuideAssignment
                {
                    TourId = request.TourId,
                    DepartureDateId = request.DepartureDateId,
                    TourGuideId = tourGuide.TourGuideId,
                    AssignedDate = DateOnly.FromDateTime(DateTime.Now),
                    IsLeadGuide = tourGuide.IsLeadGuide,
                    IsActive = true
                };
                assignments.Add(assignment);
            }

            if (assignments.Any())
            {
                _context.TourGuideAssignments.AddRange(assignments);
                await _context.SaveChangesAsync();
            }

            return Ok(new { 
                message = "Assignments created successfully", 
                createdCount = assignments.Count,
                totalRequested = request.TourGuides.Count
            });
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
                    //a.BookingId,
                    a.AssignedDate,
                    a.IsLeadGuide
                })
                .ToListAsync();
            return Ok(assignments);
        }

        [HttpGet("departure-date/{departureDateId}")]
        public async Task<IActionResult> GetAssignmentsByDepartureDate(int departureDateId)
        {
            var assignments = await _context.TourGuideAssignments
                .Include(a => a.TourGuide)
                .ThenInclude(tg => tg.User)
                .Include(a => a.DepartureDate)
                .Include(a => a.TourGuide)
                .ThenInclude(tg => tg.GuideLanguages)
                .ThenInclude(gl => gl.Language)
                .Where(a => a.DepartureDateId == departureDateId && a.IsActive)
                .Select(a => new {
                    a.Id,
                    a.TourId,
                    a.DepartureDateId,
                    a.TourGuideId,
                    a.AssignedDate,
                    a.IsLeadGuide,
                    a.IsActive,
                    TourGuideName = a.TourGuide.User.UserName,
                    TourGuideEmail = a.TourGuide.User.Email,
                    TourGuidePhone = a.TourGuide.User.PhoneNumber,
                    Languages = a.TourGuide.GuideLanguages
                        .Where(gl => gl.IsActive)
                        .Select(gl => gl.Language.LanguageName)
                        .ToList(),
                    DepartureDate = a.DepartureDate.DepartureDate1
                })
                .ToListAsync();

            return Ok(assignments);
        }

        [HttpDelete("{assignmentId}")]
        public async Task<IActionResult> DeleteAssignment(int assignmentId)
        {
            var assignment = await _context.TourGuideAssignments
                .FirstOrDefaultAsync(a => a.Id == assignmentId && a.IsActive);
            
            if (assignment == null)
            {
                return NotFound("Assignment not found");
            }

            assignment.IsActive = false;
            await _context.SaveChangesAsync();
            
            return Ok("Assignment deleted successfully");
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

    //    [HttpGet]
    //    [Authorize(Roles = "Tour Operator")]
    //    [ProducesResponseType(typeof(TourGuideAssignmentListResponse), 200)]
    //    public async Task<IActionResult> GetAssignments([FromQuery] TourGuideAssignmentSearchRequest request)
    //    {
    //        var query = _context.TourGuideAssignments
    //            .Include(a => a.TourGuide)
    //            .ThenInclude(tg => tg.User)
    //            .Include(a => a.Booking)
    //            .ThenInclude(b => b.User)
    //            .Include(a => a.Booking)
    //            .ThenInclude(b => b.Tour)
    //            .AsQueryable();

    //        // Apply filters
    //        if (request.TourId.HasValue)
    //            query = query.Where(a => a.TourId == request.TourId.Value);

    //        if (request.TourGuideId.HasValue)
    //            query = query.Where(a => a.TourGuideId == request.TourGuideId.Value);

    //        if (request.BookingId.HasValue)
    //            query = query.Where(a => a.BookingId == request.BookingId.Value);

    //        if (request.IsLeadGuide.HasValue)
    //            query = query.Where(a => a.IsLeadGuide == request.IsLeadGuide.Value);

    //        if (request.IsActive.HasValue)
    //            query = query.Where(a => a.IsActive == request.IsActive.Value);

    //        if (request.AssignedDateFrom.HasValue)
    //            query = query.Where(a => a.AssignedDate >= DateOnly.FromDateTime(request.AssignedDateFrom.Value));

    //        if (request.AssignedDateTo.HasValue)
    //            query = query.Where(a => a.AssignedDate <= DateOnly.FromDateTime(request.AssignedDateTo.Value));

    //        // Get total count for pagination
    //        var totalCount = await query.CountAsync();

    //        // Apply pagination
    //        var assignments = await query
    //            .Skip((request.PageNumber - 1) * request.PageSize)
    //            .Take(request.PageSize)
    //            .Select(a => new TourGuideAssignmentResponse
    //            {
    //                Id = a.Id,
    //                TourId = a.TourId,
    //                BookingId = a.BookingId,
    //                TourGuideId = a.TourGuideId,
    //                AssignedDate = a.AssignedDate,
    //                NoteId = a.NoteId,
    //                IsLeadGuide = a.IsLeadGuide,
    //                IsActive = a.IsActive,
    //                //TourName = a.Booking.Tour.TourType,
    //                GuideName = a.TourGuide.User.UserName,
    //                CustomerName = a.Booking.User.UserName
    //            })
    //            .ToListAsync();

    //        var response = new TourGuideAssignmentListResponse
    //        {
    //            Assignments = assignments,
    //            TotalCount = totalCount,
    //            PageNumber = request.PageNumber,
    //            PageSize = request.PageSize
    //        };

    //        return Ok(response);
    //    }
    }
} 