using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Response.TourBookingResponse;
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
                .Include(b => b.Payments)
                .Include(b => b.TourAcceptanceReports)
                .FirstOrDefault(b => b.BookingId == bookingId && b.IsActive);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            var response = new TourBookingDetailResponse
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                TourId = booking.TourId,
                //SelectedDepartureDate = booking.SelectedDepartureDate,
                BookingDate = booking.BookingDate,
                NumberOfAdults = booking.NumberOfAdults,
                NumberOfChildren = booking.NumberOfChildren,
                NumberOfInfants = booking.NumberOfInfants,
                NoteForTour = booking.NoteForTour,
                TotalPrice = booking.TotalPrice,
                //DepositAmount = booking.DepositAmount,
                //RemainingAmount = booking.RemainingAmount,
                Contract = booking.Contract,
                BookingStatus = booking.BookingStatus,
                PaymentStatus = booking.PaymentStatus,

                Payments = booking.Payments
                    .Where(p => p.IsActive)
                    .Select(p => new PaymentDto
                    {
                        PaymentId = p.PaymentId,
                        Amount = p.Amount,
                        AmountPaid = p.AmountPaid,
                        PaymentMethod = p.PaymentMethod,
                        PaymentStatus = p.PaymentStatus,
                        PaymentDate = p.PaymentDate,
                        PaymentReference = p.PaymentReference
                    }).ToList(),

                AcceptanceReport = booking.TourAcceptanceReports
                    .Where(r => r.IsActive)
                    .Select(r => new TourAcceptanceReportDto
                    {
                        ReportId = r.ReportId,
                        TourGuideId = r.TourGuideId,
                        ReportDate = r.ReportDate,
                        Summary = r.Summary,
                        TotalExtraCost = r.TotalExtraCost,
                        Notes = r.Notes,
                        AttachmentUrl = r.AttachmentUrl
                    }).FirstOrDefault(),

                ExtraCharges = context.ExtraCharges
                    .Where(ec => ec.IsActive)
                    .Select(ec => new ExtraChargeDto
                    {
                        ExtraChargeId = ec.ExtraChargeId,
                        Name = ec.Name,
                        Description = ec.Description,
                        Amount = ec.Amount
                    }).ToList()
            };

            return Ok(response);
        }

    }
}
