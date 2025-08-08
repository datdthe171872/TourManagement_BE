using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Response.PaymentResponse;

namespace TourManagement_BE.Service.PaymentHistory
{
    public class PaymentService : IPaymentService
    {
        private readonly MyDBContext _context;
        private readonly IMapper _mapper;

        public PaymentService(MyDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<ViewAllPaymentHistory>> GetAllUserPaymentHistoryAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Payments
                .Where(u => u.User.Role.RoleName == "Customer");

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderByDescending(p => p.PaymentDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ViewAllPaymentHistory>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<ViewAllPaymentHistory>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = data
            };
        }

        public async Task<PagedResult<ViewAllPaymentHistory>> SearchAllUserPaymentHistoryAsync(string keyword, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Payments
                .Where(p => p.User.Role.RoleName == "Customer");

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

            return new PagedResult<ViewAllPaymentHistory>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = data
            };
        }

        public async Task<PagedResult<ViewHistoryPaymentPackageResponse>> GetAllTourOperatorPaymentHistoryAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.PurchaseTransactions.AsQueryable();

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

            return new PagedResult<ViewHistoryPaymentPackageResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = payment
            };
        }

        public async Task<PagedResult<ViewHistoryPaymentPackageResponse>> SearchAllTourOperatorPaymentHistoryAsync(string keyword, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.PurchaseTransactions.AsQueryable();

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

            return new PagedResult<ViewHistoryPaymentPackageResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = data
            };
        }

        public async Task<PagedResult<ViewHistoryPaymentPackageResponse>> GetPaymentPackageHistoryByTourOperatorAsync(int userId, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.PurchaseTransactions
                .Include(t => t.TourOperator)
                .ThenInclude(to => to.User)
                .Where(p => p.TourOperator.UserId == userId);

            var totalRecords = await query.CountAsync();

            var history = await query
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

            return new PagedResult<ViewHistoryPaymentPackageResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = history
            };
        }

        public async Task<PagedResult<ViewPaymentResponse>> GetUserPaymentDetailHistoryAsync(int userId, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Payments
                .Where(p => p.UserId == userId && p.IsActive);

            var totalRecords = await query.CountAsync();

            var payments = await query
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
                }).ToListAsync();

            return new PagedResult<ViewPaymentResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = payments
            };
        }
    }
}
