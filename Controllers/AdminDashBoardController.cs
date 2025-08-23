using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.AccountRequest;
using TourManagement_BE.Data.DTO.Response.AccountResponse;
using TourManagement_BE.Data.DTO.Response.AdminDashBoard;
using TourManagement_BE.Data.DTO.Response.PaymentResponse;
using TourManagement_BE.Data.DTO.Response.TourResponse;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDashBoardController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;
        public AdminDashBoardController(MyDBContext context, IMapper mapper)
        {
            this.context = context;
            _mapper = mapper;
        }

        [HttpGet("ServicePackageStatistics")]
        public IActionResult GetServicePackageStatistics()
        {
            var data = context.PurchaseTransactions
            .Where(p => p.IsActive && p.PaymentStatus == "Completed")
            .GroupBy(p => new { Year = p.CreatedAt.Value.Year, Month = p.CreatedAt.Value.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                MonthNum = g.Key.Month,
                TotalRevenue = g.Sum(x => x.Amount),
                Count = g.Count()
            })
            .AsEnumerable() 
            .Select(g => new ServicePackageStatisticsResponse
            {
                Month = $"{g.Year}-{g.MonthNum:D2}", 
                TotalRevenue = g.TotalRevenue,
                PurchaseCount = g.Count
            })
            .OrderBy(x => x.Month)
            .ToList();

            return Ok(data);
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
                    EndDate = p.PurchasedServicePackages
                        .Select(psp => (DateTime?)psp.EndDate)
                        .FirstOrDefault(),
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
                query = query.Where(p => p.TourOperator.User.UserName.Contains(keyword));
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
                    EndDate = p.PurchasedServicePackages
                        .Select(psp => (DateTime?)psp.EndDate)
                        .FirstOrDefault(),
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
                    EndDate = p.PurchasedServicePackages
                        .Select(psp => (DateTime?)psp.EndDate)
                        .FirstOrDefault(),
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

        [HttpGet("PagingAllAccount")]
        public IActionResult PagingAllAccount(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalRecords = context.Users.Count();

            var users = context.Users
                .OrderBy(u => u.UserId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new ListAccountResponse
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    Address = u.Address,
                    PhoneNumber = u.PhoneNumber,
                    Avatar = u.Avatar,
                    RoleName = u.Role.RoleName,
                    IsActive = u.IsActive
                })
                .ToList();

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = users
            });
        }

        [HttpGet("PagingSearchAccount")]
        public IActionResult PagingSearchAccount(string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(keyword)
                );
            }

            var totalRecords = query.Count();

            var users = query
                .OrderBy(u => u.UserId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new ListAccountResponse
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    Address = u.Address,
                    PhoneNumber = u.PhoneNumber,
                    Avatar = u.Avatar,
                    RoleName = u.Role.RoleName,
                    IsActive = u.IsActive
                })
                .ToList();

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = users
            });
        }

        [HttpPut("UpdateStatus")]
        public IActionResult UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            var user = context.Users.FirstOrDefault(u => u.UserId == request.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.IsActive = request.IsActive;
            context.SaveChanges();

            return Ok(new { message = "User status updated successfully." });
        }

        [HttpPatch("UpdateAccountStatus/{userid}")]
        public async Task<IActionResult> UpdateAccountStatus(int userid)
        {
            var user = await context.Users.FindAsync(userid);
            if (user == null)
                return NotFound("TourMedia not found.");

            user.IsActive = !user.IsActive;

            await context.SaveChangesAsync();

            return Ok(new
            {
                message = $"User has been {(user.IsActive ? "activated" : "deactivated")}",
            });
        }


        [HttpGet("ListAllToursPaging")]
        public async Task<IActionResult> ListAllToursPaging(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalRecords = await context.Tours.CountAsync();

            var tours = await context.Tours
                .OrderBy(t => t.TourId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = tours
            });
        }

        [HttpGet("Search Tour Paging By Name")]
        public async Task<IActionResult> SearchTourPaging(string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = context.Tours.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u => u.Title.ToLower().Contains(keyword));
            }

            var totalRecords = await query.CountAsync();

            var tours = await query
                .OrderBy(t => t.TourId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!tours.Any())
            {
                return NotFound("No tours found.");
            }

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = tours
            });
        }

    }
}
