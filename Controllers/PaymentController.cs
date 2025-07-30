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
        public async Task<IActionResult> ViewAllUserPaymentHistory(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = context.Payments
                .Where(u => u.User.Role.RoleName == "Customer");

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderByDescending(p => p.PaymentDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ViewAllPaymentHistory>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!data.Any())
            {
                return NotFound("No payment history found.");
            }

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = data
            });
        }


        [HttpGet("SearchAllUserPaymentHistory")]
        public async Task<IActionResult> SearchAllUserPaymentHistory(string? keyword = "", int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = context.Payments
                .Where(p => p.User.Role.RoleName == "Customer");

            // Tìm kiếm theo tên
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p => p.User.UserName.Contains(keyword));
            }

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderByDescending(p => p.PaymentDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ViewAllPaymentHistory>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = data
            });
        }

        [HttpGet("ViewAllTourOperatorPaymentHistory")]
        public async Task<IActionResult> ViewAllTourOperatorPaymentHistory(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = context.PurchaseTransactions.AsQueryable();

            var totalRecords = await query.CountAsync();

            var payment = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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
                }).ToListAsync();

            if (!payment.Any())
            {
                return NotFound("Not Found.");
            }

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = payment
            });
        }



        [HttpGet("SearchAllTourOperatorPaymentHistory")]
        public async Task<IActionResult> SearchAllTourOperatorPaymentHistory(string? keyword = "", int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = context.PurchaseTransactions.AsQueryable();

            // Tìm kiếm theo tên tour operator
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p => p.TourOperator.CompanyName.Contains(keyword));
            }

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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
                    IsActive = p.IsActive
                })
                .ToListAsync();

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = data
            });
        }



        [HttpGet("ViewPaymentPackageHistory/{userid}")]
        public IActionResult ViewPaymentPackageHistory(int userid, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = context.PurchaseTransactions.Include(t => t.TourOperator).ThenInclude(to => to.User)
                .Where(p => p.TourOperator.UserId == userid);

            var totalRecords = query.Count();

            var history = query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = history
            });
        }

        [HttpGet("ViewUserPaymentDetailHistory/{userId}")]
        public IActionResult ViewUserPaymentDetailHistory(int userId, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = context.Payments
                .Where(p => p.UserId == userId && p.IsActive);

            var totalRecords = query.Count();

            var payments = query
                .OrderByDescending(p => p.PaymentDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = payments
            });
        }

    }
}
