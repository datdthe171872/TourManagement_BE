using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Response.TourResponse;
using TourManagement_BE.Data.DTO.Response.TourResponse.TourDetailDTO;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;

        public TourController(MyDBContext context, IMapper mapper)
        {
            this.context = context;
            _mapper = mapper;
        }

        [HttpGet("listAllTours")]
        public IActionResult ListAllTours()
        {
            var tours = context.Tours
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToList();

            if (!tours.Any())
            {
                return NotFound("Not Found.");
            }

            return Ok(tours);
        }

        [HttpGet("ListAllToursPaging")]
        public IActionResult ListAllToursPaging(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalRecords = context.Tours.Count();

            var tours = context.Tours
                .OrderBy(t => t.TourId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToList();

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = tours
            });
        }

        [HttpGet("Search Tour By Name")]
        public IActionResult SearchTour(string? keyword)
        {
            var query = context.Tours.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u => u.Title.ToLower().Contains(keyword));
            }

            var tours = query
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToList();

            if (!tours.Any())
            {
                return NotFound("No tours found.");
            }

            return Ok(tours);
        }

        [HttpGet("Search Tour Paging By Name")]
        public IActionResult SearchTourPaging(string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = context.Tours.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u => u.Title.ToLower().Contains(keyword));
            }

            var totalRecords = query.Count();

            var tours = query
                .OrderBy(t => t.TourId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToList();

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

        [HttpGet("FilterToursPaging")]
        public IActionResult FilterToursPaging(
            string? title,
            string? tourType,
            string? transportation,
            string? startPoint,
            decimal? minPrice,
            decimal? maxPrice,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = context.Tours.AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(t => t.Title.Contains(title));

            if (!string.IsNullOrEmpty(tourType))
                query = query.Where(t => t.TourType.Contains(tourType));

            if (!string.IsNullOrEmpty(transportation))
                query = query.Where(t => t.Transportation!.Contains(transportation));

            if (!string.IsNullOrEmpty(startPoint))
                query = query.Where(t => t.StartPoint!.Contains(startPoint));

            if (minPrice.HasValue)
                query = query.Where(t => t.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(t => t.Price <= maxPrice.Value);

            var totalRecords = query.Count();

            var result = query
                .OrderBy(t => t.TourId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToList();

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = result
            });
        }

        [HttpGet("FilterTours")]
        public IActionResult FilterTours(
            string? title,
            string? tourType,
            string? transportation,
            string? startPoint,
            decimal? minPrice,
            decimal? maxPrice)
        {
            var query = context.Tours.AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(t => t.Title.Contains(title));

            if (!string.IsNullOrEmpty(tourType))
                query = query.Where(t => t.TourType.Contains(tourType));

            if (!string.IsNullOrEmpty(transportation))
                query = query.Where(t => t.Transportation!.Contains(transportation));

            if (!string.IsNullOrEmpty(startPoint))
                query = query.Where(t => t.StartPoint!.Contains(startPoint));

            if (minPrice.HasValue)
                query = query.Where(t => t.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(t => t.Price <= maxPrice.Value);

            var result = query
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToList();

            return Ok(result);
        }

        [HttpGet("TourDetail/{tourid}")]
        public IActionResult TourDetail(int tourid)
        {
            var tour = context.Tours
                .Include(x => x.TourOperator).ThenInclude(op => op.User)
                .Include(x => x.DepartureDates)
                .Include(x => x.TourExperiences)
                .Include(x => x.TourItineraries).ThenInclude(ti => ti.ItineraryMedia)
                .Include(x => x.TourMedia)
                .Include(x => x.TourRatings).ThenInclude(r => r.User)
                .FirstOrDefault(t => t.TourId == tourid);

            if (tour == null)
            {
                return NotFound("Tour not found.");
            }
            var result = _mapper.Map<TourDetailResponse>(tour);
            return Ok(result);
        }
    }
}
