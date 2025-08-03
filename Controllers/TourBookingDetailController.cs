using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Response.TourBookingDetailResponse;
using TourManagement_BE.Data.DTO.Response.TourBookingResponse;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.TourManagement_BE.Data.DTO.Response.TourBookingResponse;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourBookingDetailController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;

        public TourBookingDetailController(MyDBContext context, IMapper mapper)
        {
            this.context = context;
            _mapper = mapper;
        }

        [HttpGet("ViewBookingDetail/{bookingId}")]
        public IActionResult ViewBookingDetail(int bookingId)
        {
            var booking = context.Bookings
                .Include(b => b.User)
                .Include(b => b.DepartureDate)
                .Include(b => b.Payments)
                    .ThenInclude(p => p.User)
                .Include(b => b.Payments)
                    .ThenInclude(p => p.PaymentType)
                .Include(b => b.TourAcceptanceReports)
                //.Include(b => b.BookingExtraCharges)
                    //.ThenInclude(bec => bec.ExtraCharge)
                .FirstOrDefault(b => b.BookingId == bookingId && b.IsActive);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            var response = _mapper.Map<TourBookingDetailResponse>(booking);
            
            // Populate GuideNotes manually
            var guideNotes = context.TourGuideAssignments
                .Where(tga => tga.TourId == booking.TourId && 
                             tga.DepartureDateId == booking.DepartureDateId && 
                             tga.IsActive)
                .SelectMany(tga => tga.GuideNotes.Where(gn => gn.IsActive))
                .Select(gn => new GuideNotesInfo
                {
                    NoteId = gn.NoteId,
                    Title = gn.Title,
                    Content = gn.Content,
                    ExtraCost = gn.ExtraCost,
                    CreatedAt = gn.CreatedAt
                })
                .ToList();
            
            response.GuideNotes = guideNotes;
            return Ok(response);
        }

    }
}
