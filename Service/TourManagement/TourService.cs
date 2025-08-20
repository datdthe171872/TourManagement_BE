using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.TourRequest;
using TourManagement_BE.Data.DTO.Response.TourResponse;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Repository.Interface;

namespace TourManagement_BE.Service.TourManagement
{
    public class TourService : ITourService
    {
        private readonly IMapper _mapper;
        private readonly MyDBContext _context;

        public TourService(MyDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ListTourResponse>> ListAllForTourOperatorAsync(int userId)
        {
            var tours = await _context.Tours
            .Include(t => t.TourOperator)
            .ThenInclude(to => to.User)
            .Where(t => t.TourOperator.User.UserId == userId)
            .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();

            return tours;
        }

        public async Task<PagedResult<ListTourResponse>> ListAllForTourOperatorPagingAsync(int userId, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Tours
                .Include(t => t.TourOperator)
                .ThenInclude(to => to.User)
                .Where(t => t.TourOperator.User.UserId == userId);

            var totalRecords = await query.CountAsync();

            var tours = await query
                .OrderBy(t => t.TourId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<ListTourResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = tours
            };
        }

        public async Task<PagedResult<ListTourResponse>> TourOperatorFullTourListPagingAsync(int touroperatorid, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Tours
                .Include(t => t.TourOperator)
                .ThenInclude(to => to.User)
                .Where(t => t.TourOperatorId == touroperatorid);

            var totalRecords = await query.CountAsync();

            var tours = await query
                .OrderBy(t => t.TourId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<ListTourResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = tours
            };
        }

        public async Task<List<ListTourResponse>> SearchForOperatorAsync(int userId, string keyword)
        {
            var query = _context.Tours
                .Include(t => t.TourOperator)
                .ThenInclude(to => to.User)
                .Where(t => t.TourOperator.User.UserId == userId);

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u => u.Title.ToLower().Contains(keyword));
            }

            return await query
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<PagedResult<ListTourResponse>> SearchPagingForOperatorAsync(int userId, string keyword, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Tours
                .Include(t => t.TourOperator)
                .ThenInclude(to => to.User)
                .Where(t => t.TourOperator.User.UserId == userId);

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

            return new PagedResult<ListTourResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = tours
            };
        }

        public async Task<PagedResult<ListTourResponse>> SearchPagingTourOperatorFullTourListAsync(int touroperatorid, string keyword, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Tours
                .Include(t => t.TourOperator)
                .ThenInclude(to => to.User)
                .Where(t => t.TourOperatorId == touroperatorid);

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

            return new PagedResult<ListTourResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = tours
            };
        }

        public async Task<PagedResult<ListTourResponse>> FilterForOperatorPagingAsync(int userId, TourFilterRequest filter, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Tours
                .Include(t => t.TourOperator)
                .ThenInclude(to => to.User)
                .Where(t => t.TourOperator.User.UserId == userId);

            if (!string.IsNullOrEmpty(filter.Title))
                query = query.Where(t => t.Title.Contains(filter.Title));

            if (!string.IsNullOrEmpty(filter.Transportation))
                query = query.Where(t => t.Transportation!.Contains(filter.Transportation));

            if (!string.IsNullOrEmpty(filter.StartPoint))
                query = query.Where(t => t.StartPoint!.Contains(filter.StartPoint));

            if (filter.MinPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren <= filter.MaxPrice.Value);

            if (filter.Ratings != null && filter.Ratings.Length > 0)
            {
                query = query.Where(t =>
                    filter.Ratings.Contains(
                        (int)Math.Round(
                            t.TourRatings.Any()
                            ? t.TourRatings.Average(r => (double?)r.Rating) ?? 0
                            : 0
                        )
                    )
                );
            }

            var totalRecords = await query.CountAsync();

            var result = await query
                .OrderBy(t => t.TourId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<ListTourResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = result
            };
        }

        public async Task<PagedResult<ListTourResponse>> FilterPagingTourOperatorFullTourListAsync(int touroperatorid, TourFilterRequest filter, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Tours
                .Include(t => t.TourOperator)
                .ThenInclude(to => to.User)
                .Where(t => t.TourOperatorId == touroperatorid);

            if (!string.IsNullOrEmpty(filter.Title))
                query = query.Where(t => t.Title.Contains(filter.Title));

            if (!string.IsNullOrEmpty(filter.Transportation))
                query = query.Where(t => t.Transportation!.Contains(filter.Transportation));

            if (!string.IsNullOrEmpty(filter.StartPoint))
                query = query.Where(t => t.StartPoint!.Contains(filter.StartPoint));

            if (filter.MinPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren <= filter.MaxPrice.Value);

            if (filter.Ratings != null && filter.Ratings.Length > 0)
            {
                query = query.Where(t =>
                    filter.Ratings.Contains(
                        (int)Math.Round(
                            t.TourRatings.Any()
                            ? t.TourRatings.Average(r => (double?)r.Rating) ?? 0
                            : 0
                        )
                    )
                );
            }

            var totalRecords = await query.CountAsync();

            var result = await query
                .OrderBy(t => t.TourId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<ListTourResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = result
            };
        }

        public async Task<List<ListTourResponse>> FilterForOperatorAsync(int userId, TourFilterRequest filter)
        {
            var query = _context.Tours
                .Include(t => t.TourOperator)
                .ThenInclude(to => to.User)
                .Where(t => t.TourOperator.User.UserId == userId);

            if (!string.IsNullOrEmpty(filter.Title))
                query = query.Where(t => t.Title.Contains(filter.Title));

            if (!string.IsNullOrEmpty(filter.Transportation))
                query = query.Where(t => t.Transportation!.Contains(filter.Transportation));

            if (!string.IsNullOrEmpty(filter.StartPoint))
                query = query.Where(t => t.StartPoint!.Contains(filter.StartPoint));

            if (filter.MinPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren <= filter.MaxPrice.Value);

            if (filter.Ratings != null && filter.Ratings.Length > 0)
            {
                query = query.Where(t =>
                    filter.Ratings.Contains(
                        (int)Math.Round(
                            t.TourRatings.Any()
                            ? t.TourRatings.Average(r => (double?)r.Rating) ?? 0
                            : 0
                        )
                    )
                );
            }

            return await query
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<TourDetailResponse> GetDetailForOperatorAsync(int tourId, bool forUpdate = false)
        {
            var tour = await _context.Tours
                .Include(x => x.TourOperator).ThenInclude(op => op.User)
                .Include(x => x.DepartureDates)
                .Include(x => x.TourExperiences)
                .Include(x => x.TourItineraries).ThenInclude(ti => ti.ItineraryMedia)
                .Include(x => x.TourMedia)
                .Include(x => x.TourRatings).ThenInclude(r => r.User)
                .FirstOrDefaultAsync(t => t.TourId == tourId);

            if (tour == null)
            {
                return null;
            }

            if (forUpdate)
            {
                var today = DateTime.Today;
                tour.DepartureDates = tour.DepartureDates
                    .Where(d => d.DepartureDate1 > today)
                    .ToList();
            }

            return _mapper.Map<TourDetailResponse>(tour);
        }

        public async Task<List<ListTourResponse>> ListAllForCustomerAsync()
        {
            return await _context.Tours
                .Where(t => t.IsActive == true)
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<PagedResult<ListTourResponse>> ListAllForCustomerPagingAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Tours.Where(t => t.IsActive == true);

            var totalRecords = await query.CountAsync();

            var tours = await query
                .OrderBy(t => t.TourId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<ListTourResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = tours
            };
        }

        public async Task<List<ListTourResponse>> SearchForCustomerAsync(string keyword)
        {
            var query = _context.Tours.Where(t => t.IsActive == true);

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u => u.Title.ToLower().Contains(keyword));
            }

            return await query
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<PagedResult<ListTourResponse>> SearchPagingForCustomerAsync(string keyword, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Tours.Where(t => t.IsActive == true);

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

            return new PagedResult<ListTourResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = tours
            };
        }

        public async Task<PagedResult<ListTourResponse>> FilterForCustomerPagingAsync(TourFilterRequest filter, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Tours.Where(t => t.IsActive == true);

            if (!string.IsNullOrEmpty(filter.Title))
                query = query.Where(t => t.Title.Contains(filter.Title));

            if (!string.IsNullOrEmpty(filter.Transportation))
                query = query.Where(t => t.Transportation!.Contains(filter.Transportation));

            if (!string.IsNullOrEmpty(filter.StartPoint))
                query = query.Where(t => t.StartPoint!.Contains(filter.StartPoint));

            if (filter.MinPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren <= filter.MaxPrice.Value);

            if (filter.Ratings != null && filter.Ratings.Length > 0)
            {
                query = query.Where(t =>
                    filter.Ratings.Contains(
                        (int)Math.Round(
                            t.TourRatings.Any()
                            ? t.TourRatings.Average(r => (double?)r.Rating) ?? 0
                            : 0
                        )
                    )
                );
            }

            var totalRecords = await query.CountAsync();

            var result = await query
                .OrderBy(t => t.TourId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<ListTourResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = result
            };
        }

        public async Task<List<ListTourResponse>> FilterForCustomerAsync(TourFilterRequest filter)
        {
            var query = _context.Tours.Where(t => t.IsActive == true);

            if (!string.IsNullOrEmpty(filter.Title))
                query = query.Where(t => t.Title.Contains(filter.Title));

            if (!string.IsNullOrEmpty(filter.Transportation))
                query = query.Where(t => t.Transportation.Contains(filter.Transportation));

            if (!string.IsNullOrEmpty(filter.StartPoint))
                query = query.Where(t => t.StartPoint.Contains(filter.StartPoint));

            if (filter.MinPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren <= filter.MaxPrice.Value);

            if (filter.Ratings != null && filter.Ratings.Length > 0)
            {
                query = query.Where(t =>
                    filter.Ratings.Contains(
                        (int)Math.Round(
                            t.TourRatings.Any()
                            ? t.TourRatings.Average(r => (double?)r.Rating) ?? 0
                            : 0
                        )
                    )
                );
            }

            return await query
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<TourDetailResponse> GetDetailForCustomerAsync(int tourId)
        {
            var tour = await _context.Tours
                .Include(x => x.TourOperator).ThenInclude(op => op.User)
                .Include(x => x.DepartureDates)
                .Include(x => x.TourExperiences)
                .Include(x => x.TourItineraries).ThenInclude(ti => ti.ItineraryMedia)
                .Include(x => x.TourMedia)
                .Include(x => x.TourRatings).ThenInclude(r => r.User)
                .Where(t => t.IsActive == true)
                .FirstOrDefaultAsync(t => t.TourId == tourId);

            if (tour == null)
            {
                return null;
            }

            var today = DateTime.UtcNow.AddHours(7).Date;
            tour.DepartureDates = tour.DepartureDates
                .Where(d => d.DepartureDate1.Date > today && d.IsActive)
                .ToList();

            return _mapper.Map<TourDetailResponse>(tour);
        }



        /*public async Task<ToggleStatusResult> ToggleStatusAsync(int tourId)
        {
            var tour = await _context.Tours.FindAsync(tourId)
                ?? throw new KeyNotFoundException("Tour not found.");

            tour.IsActive = !tour.IsActive;
            tour.TourStatus = tour.IsActive ? "Active" : "InActive";

            await _context.SaveChangesAsync();

            return new ToggleStatusResult
            {
                Message = $"Tour has been {(tour.IsActive ? "activated" : "deactivated")}",
                NewStatus = tour.IsActive,
                TourStatus = tour.TourStatus
            };
        }*/

        public async Task<ToggleStatusResult> ToggleStatusAsync(int tourId)
        {
            var tour = await _context.Tours.FindAsync(tourId)
                ?? throw new KeyNotFoundException("Tour not found.");

            if (tour.IsActive)
            {
                bool hasBooking = await _context.Bookings
                    .AnyAsync(b => b.TourId == tourId && b.IsActive);

                if (hasBooking)
                {
                    return new ToggleStatusResult
                    {
                        Message = "Không thể hủy kích hoạt tour vì đã có khách đặt.",
                        NewStatus = tour.IsActive,
                        TourStatus = tour.TourStatus
                    };
                }
            }

            // Đổi trạng thái
            tour.IsActive = !tour.IsActive;
            tour.TourStatus = tour.IsActive ? "Active" : "InActive";

            await _context.SaveChangesAsync();

            return new ToggleStatusResult
            {
                Message = $"Tour has been {(tour.IsActive ? "activated" : "deactivated")}",
                NewStatus = tour.IsActive,
                TourStatus = tour.TourStatus
            };
        }

    }
}
