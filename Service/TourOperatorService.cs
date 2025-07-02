using Data.DTO.Response;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Service;

public class TourOperatorService : ITourOperatorService
{
    private readonly MyDBContext _context;

    public TourOperatorService(MyDBContext context)
    {
        _context = context;
    }

    public async Task<TourOperatorListResponse> GetTourOperatorsAsync(TourOperatorSearchRequest request)
    {
        var query = _context.TourOperators
            .Where(to => to.IsActive)
            .Include(to => to.TourOperatorMedia.Where(tom => tom.IsActive))
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.CompanyName))
        {
            query = query.Where(to => to.CompanyName != null &&
                                    to.CompanyName.Contains(request.CompanyName));
        }

        // Get total count for pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var tourOperators = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(to => new TourOperatorResponse
            {
                TourOperatorId = to.TourOperatorId,
                CompanyName = to.CompanyName,
                Description = to.Description,
                CompanyLogo = to.CompanyLogo,
                Address = to.Address,
                IsActive = to.IsActive,
                Media = to.TourOperatorMedia.Select(tom => new TourOperatorMediaResponse
                {
                    Id = tom.Id,
                    TourOperatorId = tom.TourOperatorId,
                    MediaUrl = tom.MediaUrl,
                    Caption = tom.Caption,
                    UploadedAt = tom.UploadedAt,
                    IsActive = tom.IsActive
                }).ToList()
            })
            .ToListAsync();

        // Calculate pagination info
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new TourOperatorListResponse
        {
            TourOperators = tourOperators,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = totalPages,
            HasNextPage = request.PageNumber < totalPages,
            HasPreviousPage = request.PageNumber > 1
        };
    }

    public async Task<TourOperatorDetailResponse?> GetTourOperatorDetailAsync(int id)
    {
        var to = await _context.TourOperators
            .Include(t => t.TourOperatorMedia.Where(tom => tom.IsActive))
            .FirstOrDefaultAsync(x => x.TourOperatorId == id);
            
        if (to == null) return null;
        
        return new TourOperatorDetailResponse
        {
            TourOperatorId = to.TourOperatorId,
            UserId = to.UserId,
            CompanyName = to.CompanyName,
            Description = to.Description,
            CompanyLogo = to.CompanyLogo,
            LicenseNumber = to.LicenseNumber,
            LicenseIssuedDate = to.LicenseIssuedDate,
            TaxCode = to.TaxCode,
            EstablishedYear = to.EstablishedYear,
            Hotline = to.Hotline,
            Website = to.Website,
            Facebook = to.Facebook,
            Instagram = to.Instagram,
            Address = to.Address,
            WorkingHours = to.WorkingHours,
            IsActive = to.IsActive,
            Media = to.TourOperatorMedia.Select(tom => new TourOperatorMediaResponse
            {
                Id = tom.Id,
                TourOperatorId = tom.TourOperatorId,
                MediaUrl = tom.MediaUrl,
                Caption = tom.Caption,
                UploadedAt = tom.UploadedAt,
                IsActive = tom.IsActive
            }).ToList()
        };
    }

    public async Task<TourOperatorDetailResponse> CreateTourOperatorAsync(CreateTourOperatorRequest request)
    {
        // Kiểm tra xem UserId đã tồn tại tour operator chưa
        var existingTourOperator = await _context.TourOperators
            .FirstOrDefaultAsync(to => to.UserId == request.UserId && to.IsActive);
        
        if (existingTourOperator != null)
        {
            throw new InvalidOperationException("User này đã có tour operator rồi.");
        }

        // Kiểm tra xem tên công ty đã tồn tại chưa
        var existingCompanyName = await _context.TourOperators
            .FirstOrDefaultAsync(to => to.CompanyName == request.CompanyName && to.IsActive);
        
        if (existingCompanyName != null)
        {
            throw new InvalidOperationException("Tên công ty này đã tồn tại.");
        }

        var tourOperator = new TourOperator
        {
            UserId = request.UserId,
            CompanyName = request.CompanyName,
            Description = request.Description,
            CompanyLogo = request.CompanyLogo,
            LicenseNumber = request.LicenseNumber,
            LicenseIssuedDate = request.LicenseIssuedDate.HasValue ? DateOnly.FromDateTime(request.LicenseIssuedDate.Value) : null,
            TaxCode = request.TaxCode,
            EstablishedYear = request.EstablishedYear,
            Hotline = request.Hotline,
            Website = request.Website,
            Facebook = request.Facebook,
            Instagram = request.Instagram,
            Address = request.Address,
            WorkingHours = request.WorkingHours,
            IsActive = true
        };

        _context.TourOperators.Add(tourOperator);
        await _context.SaveChangesAsync();

        // Xử lý thêm media nếu có MediaUrl
        var mediaList = new List<TourOperatorMediaResponse>();
        if (!string.IsNullOrWhiteSpace(request.MediaUrl))
        {
            var tourOperatorMedia = new TourOperatorMedia
            {
                TourOperatorId = tourOperator.TourOperatorId,
                MediaUrl = request.MediaUrl,
                Caption = "Ảnh công ty",
                UploadedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.TourOperatorMedia.Add(tourOperatorMedia);
            await _context.SaveChangesAsync();

            mediaList.Add(new TourOperatorMediaResponse
            {
                Id = tourOperatorMedia.Id,
                TourOperatorId = tourOperatorMedia.TourOperatorId,
                MediaUrl = tourOperatorMedia.MediaUrl,
                Caption = tourOperatorMedia.Caption,
                UploadedAt = tourOperatorMedia.UploadedAt,
                IsActive = tourOperatorMedia.IsActive
            });
        }

        return new TourOperatorDetailResponse
        {
            TourOperatorId = tourOperator.TourOperatorId,
            UserId = tourOperator.UserId,
            CompanyName = tourOperator.CompanyName,
            Description = tourOperator.Description,
            CompanyLogo = tourOperator.CompanyLogo,
            LicenseNumber = tourOperator.LicenseNumber,
            LicenseIssuedDate = tourOperator.LicenseIssuedDate,
            TaxCode = tourOperator.TaxCode,
            EstablishedYear = tourOperator.EstablishedYear,
            Hotline = tourOperator.Hotline,
            Website = tourOperator.Website,
            Facebook = tourOperator.Facebook,
            Instagram = tourOperator.Instagram,
            Address = tourOperator.Address,
            WorkingHours = tourOperator.WorkingHours,
            IsActive = tourOperator.IsActive,
            Media = mediaList
        };
    }

    public async Task<TourOperatorDetailResponse?> UpdateTourOperatorAsync(int id, UpdateTourOperatorRequest request)
    {
        var tourOperator = await _context.TourOperators
            .Include(t => t.TourOperatorMedia.Where(tom => tom.IsActive))
            .FirstOrDefaultAsync(to => to.TourOperatorId == id && to.IsActive);
        
        if (tourOperator == null)
        {
            return null;
        }

        // Kiểm tra xem tên công ty mới có trùng với tour operator khác không
        var existingCompanyName = await _context.TourOperators
            .FirstOrDefaultAsync(to => to.CompanyName == request.CompanyName && 
                                     to.TourOperatorId != id && 
                                     to.IsActive);
        
        if (existingCompanyName != null)
        {
            throw new InvalidOperationException("Tên công ty này đã tồn tại.");
        }

        // Cập nhật thông tin
        tourOperator.CompanyName = request.CompanyName;
        tourOperator.Description = request.Description;
        tourOperator.CompanyLogo = request.CompanyLogo;
        tourOperator.LicenseNumber = request.LicenseNumber;
        tourOperator.LicenseIssuedDate = request.LicenseIssuedDate.HasValue ? DateOnly.FromDateTime(request.LicenseIssuedDate.Value) : null;
        tourOperator.TaxCode = request.TaxCode;
        tourOperator.EstablishedYear = request.EstablishedYear;
        tourOperator.Hotline = request.Hotline;
        tourOperator.Website = request.Website;
        tourOperator.Facebook = request.Facebook;
        tourOperator.Instagram = request.Instagram;
        tourOperator.Address = request.Address;
        tourOperator.WorkingHours = request.WorkingHours;

        await _context.SaveChangesAsync();

        return new TourOperatorDetailResponse
        {
            TourOperatorId = tourOperator.TourOperatorId,
            UserId = tourOperator.UserId,
            CompanyName = tourOperator.CompanyName,
            Description = tourOperator.Description,
            CompanyLogo = tourOperator.CompanyLogo,
            LicenseNumber = tourOperator.LicenseNumber,
            LicenseIssuedDate = tourOperator.LicenseIssuedDate,
            TaxCode = tourOperator.TaxCode,
            EstablishedYear = tourOperator.EstablishedYear,
            Hotline = tourOperator.Hotline,
            Website = tourOperator.Website,
            Facebook = tourOperator.Facebook,
            Instagram = tourOperator.Instagram,
            Address = tourOperator.Address,
            WorkingHours = tourOperator.WorkingHours,
            IsActive = tourOperator.IsActive,
            Media = tourOperator.TourOperatorMedia.Select(tom => new TourOperatorMediaResponse
            {
                Id = tom.Id,
                TourOperatorId = tom.TourOperatorId,
                MediaUrl = tom.MediaUrl,
                Caption = tom.Caption,
                UploadedAt = tom.UploadedAt,
                IsActive = tom.IsActive
            }).ToList()
        };
    }

    public async Task<bool> SoftDeleteTourOperatorAsync(int id)
    {
        var tourOperator = await _context.TourOperators
            .FirstOrDefaultAsync(to => to.TourOperatorId == id && to.IsActive);
        
        if (tourOperator == null)
        {
            return false;
        }

        // Thực hiện xóa mềm bằng cách set IsActive = false
        tourOperator.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<TourOperatorDashboardResponse> GetDashboardStats(int operatorId)
    {
        var response = new TourOperatorDashboardResponse();

        // Tổng số tour của operator
        response.TotalTours = await _context.Tours.CountAsync(t => t.TourOperatorId == operatorId);

        // Lấy danh sách tourId của operator
        var tourIds = await _context.Tours
            .Where(t => t.TourOperatorId == operatorId)
            .Select(t => t.TourId)
            .ToListAsync();

        // Tổng số booking
        response.TotalBookings = await _context.Bookings.CountAsync(b => tourIds.Contains(b.TourId));

        // Tổng doanh thu (chỉ tính booking đã thanh toán)
      

        // Không thống kê feedback
        response.TotalFeedbacks = 0;

        // Điểm rating trung bình
        response.AverageRating = await _context.TourRatings
      .Where(r => tourIds.Contains(r.Tour.TourId) && r.Rating.HasValue)
      .AverageAsync(r => (double?)r.Rating) ?? 0.0;

        // Thống kê booking theo tháng (12 tháng gần nhất)
        var now = DateTime.UtcNow;
        var fromMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-11);
        var monthlyBookings = await _context.Bookings
            .Where(b => tourIds.Contains(b.TourId) && b.BookingDate.HasValue && b.BookingDate.Value >= fromMonth)
            .GroupBy(b => new { b.BookingDate.Value.Year, b.BookingDate.Value.Month })
            .Select(g => new
            {
                Month = g.Key.Year.ToString() + "-" + g.Key.Month.ToString("D2"),
                Value = g.Count()
            })
            .ToListAsync();
        response.MonthlyBookingStats = monthlyBookings
            .OrderBy(x => x.Month)
            .Select(x => new MonthlyStat { Month = x.Month, Value = x.Value })
            .ToList();

        // Thống kê doanh thu theo tháng (12 tháng gần nhất)
        var monthlyRevenue = await _context.Bookings
            .Where(b => tourIds.Contains(b.TourId) && b.PaymentStatus == "Paid" && b.BookingDate.HasValue && b.BookingDate.Value >= fromMonth)
            .GroupBy(b => new { b.BookingDate.Value.Year, b.BookingDate.Value.Month })
            .Select(g => new
            {
                Month = g.Key.Year.ToString() + "-" + g.Key.Month.ToString("D2"),
                Value = g.Sum(b => b.TotalPrice ?? 0)
            })
            .ToListAsync();
        response.MonthlyRevenueStats = monthlyRevenue
            .OrderBy(x => x.Month)
            .Select(x => new MonthlyStat { Month = x.Month, Value = (int)x.Value })
            .ToList();

        // Top 5 tour có nhiều booking nhất
        var topTours = await _context.Bookings
            .Where(b => tourIds.Contains(b.TourId))
            .GroupBy(b => b.TourId)
            .Select(g => new
            {
                TourId = g.Key,
                Bookings = g.Count()
            })
            .OrderByDescending(x => x.Bookings)
            .Take(5)
            .ToListAsync();
        var tourNames = await _context.Tours
            .Where(t => topTours.Select(tt => tt.TourId).Contains(t.TourId))
            .ToDictionaryAsync(t => t.TourId, t => t.Title);
        response.TopTours = topTours
            .Select(x => new TopTourStat
            {
                TourId = x.TourId,
                Name = tourNames.ContainsKey(x.TourId) ? tourNames[x.TourId] : "",
                Bookings = x.Bookings
            })
            .ToList();

        return response;
    }
}