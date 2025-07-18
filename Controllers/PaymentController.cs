using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Response.PaymentResponse;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;

        public PaymentController(MyDBContext context, IMapper mapper)
        {
            this.context = context;
            _mapper = mapper;
        }

        [HttpGet("ViewAllUserPaymentHistory")]
        public async Task<IActionResult> ViewAllUserPaymentHistory()
        {
            var payment = await context.Payments.Where(u => u.User.Role.RoleName == "Customer")
                .ProjectTo<ViewAllPaymentHistory>(_mapper.ConfigurationProvider).ToListAsync();

            if (!payment.Any())
            {
                return NotFound("Not Found.");
            }

            return Ok(payment);
        }

        [HttpGet("ViewAllTourOperatorPaymentHistory")]
        public async Task<IActionResult> ViewAllTourOperatorPaymentHistory()
        {
            var payment = await context.Payments.Where(u => u.User.Role.RoleName == "Tour Operator")
                .ProjectTo<ViewAllPaymentHistory>(_mapper.ConfigurationProvider).ToListAsync();

            if (!payment.Any())
            {
                return NotFound("Not Found.");
            }

            return Ok(payment);
        }

        [HttpGet("ViewPaymentPackageHistory/{tourOperatorId}")]
        public IActionResult ViewPaymentPackageHistory(int tourOperatorId)
        {
            var history = context.PurchaseTransactions
                .Where(p => p.TourOperatorId == tourOperatorId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new ViewHistoryPaymentPackageResponse
                {
                    TransactionId = p.TransactionId,
                    TourOperatorId = p.TourOperatorId,
                    TourOperatorName = p.TourOperator.User.UserName,                 
                    PackageId = p.PackageId,
                    PackageName = p.Package.Name,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus,
                    CreatedAt = p.CreatedAt,
                    IsActive = p.IsActive,
                }).ToList();

            if (!history.Any())
                return NotFound("No payment history found for this tour operator.");

            return Ok(history);
        }

        [HttpGet("ViewUserPaymentDetailHistory/{userId}")]
        public IActionResult ViewUserPaymentDetailHistory(int userId)
        {
            var payments = context.Payments
                .Where(p => p.UserId == userId && p.IsActive)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new ViewPaymentResponse
                {
                    PaymentId = p.PaymentId,
                    BookingId = p.BookingId,
                    UserId = p.UserId,
                    UserName = p.User.UserName,
                    Amount = p.Amount,
                    AmountPaid = p.AmountPaid,
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus,
                    PaymentDate = p.PaymentDate,
                    PaymentTypeId = p.PaymentTypeId,
                    PaymentTypeName = p.PaymentType.PaymentTypeName,
                    PaymentReference = p.PaymentReference,
                    IsActive = p.IsActive
                }).ToList();

            if (!payments.Any())
            {
                return NotFound("No payment history found for this user.");
            }

            return Ok(payments);
        }

    }
}
