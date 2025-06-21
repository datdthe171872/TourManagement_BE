using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;

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
                IsActive = to.IsActive
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
        var to = await _context.TourOperators.FirstOrDefaultAsync(x => x.TourOperatorId == id);
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
            IsActive = to.IsActive
        };
    }
}