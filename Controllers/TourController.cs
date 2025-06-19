using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Response.AccountResponse;
using TourManagement_BE.Data.DTO.Response.TourResponse;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourController : Controller
    {
        private readonly MyDBContext context;

        public TourController(MyDBContext context)
        {
            this.context = context;
        }

        [HttpGet("listAllTours")]
        public IActionResult ListAllTours()
        {
            var tour = context.Tours.Select(u => new ListTourResponse
            {
                TourId = u.TourId,
                Title = u.Title,
                Description = u.Description,
                Price = u.Price,
                DurationInDays = u.DurationInDays,
                StartPoint = u.StartPoint,
                Transportation = u.Transportation,
                TourOperatorId = u.TourOperatorId,
                MaxSlots = u.MaxSlots,
                SlotsBooked = u.SlotsBooked,
                CreatedAt = u.CreatedAt,
                TourType = u.TourType,
                Note = u.Note,
                TourStatus = u.TourStatus,
                IsActive = u.IsActive,
                CompanyName = u.TourOperator.CompanyName,
                CompanyDescription = u.TourOperator.Description
            });

            if (tour == null)
            {
                return NotFound("Not Found.");
            }

            return Ok(tour);
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
                .Select(u => new ListTourResponse
                {
                    TourId = u.TourId,
                    Title = u.Title,
                    Description = u.Description,
                    Price = u.Price,
                    DurationInDays = u.DurationInDays,
                    StartPoint = u.StartPoint,
                    Transportation = u.Transportation,
                    TourOperatorId = u.TourOperatorId,
                    MaxSlots = u.MaxSlots,
                    SlotsBooked = u.SlotsBooked,
                    CreatedAt = u.CreatedAt,
                    TourType = u.TourType,
                    Note = u.Note,
                    TourStatus = u.TourStatus,
                    IsActive = u.IsActive,
                    CompanyName = u.TourOperator.CompanyName,
                    CompanyDescription = u.TourOperator.Description
                })
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
                query = query.Where(u =>u.Title.ToLower().Contains(keyword));
            }

            var tours = query.
                Select(u => new ListTourResponse
            {
                TourId = u.TourId,
                Title = u.Title,
                Description = u.Description,
                Price = u.Price,
                DurationInDays = u.DurationInDays,
                StartPoint = u.StartPoint,
                Transportation = u.Transportation,
                TourOperatorId = u.TourOperatorId,
                MaxSlots = u.MaxSlots,
                SlotsBooked = u.SlotsBooked,
                CreatedAt = u.CreatedAt,
                TourType = u.TourType,
                Note = u.Note,
                TourStatus = u.TourStatus,
                IsActive = u.IsActive,
                CompanyName = u.TourOperator.CompanyName,
                CompanyDescription = u.TourOperator.Description
            }).ToList();

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

            var totalRecords = context.Tours.Count();

            var query = context.Tours.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u => u.Title.ToLower().Contains(keyword));
            }

            var tours = query.OrderBy(t => t.TourId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).
                Select(u => new ListTourResponse
                {
                    TourId = u.TourId,
                    Title = u.Title,
                    Description = u.Description,
                    Price = u.Price,
                    DurationInDays = u.DurationInDays,
                    StartPoint = u.StartPoint,
                    Transportation = u.Transportation,
                    TourOperatorId = u.TourOperatorId,
                    MaxSlots = u.MaxSlots,
                    SlotsBooked = u.SlotsBooked,
                    CreatedAt = u.CreatedAt,
                    TourType = u.TourType,
                    Note = u.Note,
                    TourStatus = u.TourStatus,
                    IsActive = u.IsActive,
                    CompanyName = u.TourOperator.CompanyName,
                    CompanyDescription = u.TourOperator.Description
                }).ToList();

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
                .Select(t => new ListTourResponse
                {
                    TourId = t.TourId,
                    Title = t.Title,
                    Description = t.Description,
                    Price = t.Price,
                    DurationInDays = t.DurationInDays,
                    StartPoint = t.StartPoint,
                    Transportation = t.Transportation,
                    TourOperatorId = t.TourOperatorId,
                    MaxSlots = t.MaxSlots,
                    SlotsBooked = t.SlotsBooked,
                    CreatedAt = t.CreatedAt,
                    TourType = t.TourType,
                    Note = t.Note,
                    TourStatus = t.TourStatus,
                    IsActive = t.IsActive,
                    CompanyName = t.TourOperator.CompanyName,
                    CompanyDescription = t.TourOperator.Description
                }).ToList();

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

            var totalRecords = query.Count();

            var result = query.Select(t => new ListTourResponse
                {
                    TourId = t.TourId,
                    Title = t.Title,
                    Description = t.Description,
                    Price = t.Price,
                    DurationInDays = t.DurationInDays,
                    StartPoint = t.StartPoint,
                    Transportation = t.Transportation,
                    TourOperatorId = t.TourOperatorId,
                    MaxSlots = t.MaxSlots,
                    SlotsBooked = t.SlotsBooked,
                    CreatedAt = t.CreatedAt,
                    TourType = t.TourType,
                    Note = t.Note,
                    TourStatus = t.TourStatus,
                    IsActive = t.IsActive,
                    CompanyName = t.TourOperator.CompanyName,
                    CompanyDescription = t.TourOperator.Description
                }).ToList();

            return Ok(result);
        }

    }
}
