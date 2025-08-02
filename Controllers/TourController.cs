using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.DepartureDatesRequest;
using TourManagement_BE.Data.DTO.Request.TourExperienceRequest;
using TourManagement_BE.Data.DTO.Request.TourItineraryRequest;
using TourManagement_BE.Data.DTO.Request.TourMediaRequest;
using TourManagement_BE.Data.DTO.Request.TourRequest;
using TourManagement_BE.Data.DTO.Response.PaymentResponse;
using TourManagement_BE.Data.DTO.Response.TourResponse;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Helper.Constant;
using TourManagement_BE.Repository.Interface;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly ISlotCheckService _slotCheckService;
        

        public TourController(MyDBContext context, IMapper mapper, Cloudinary cloudinary, ISlotCheckService slotCheckService)
        {
            this.context = context;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _slotCheckService = slotCheckService;
        }

        [HttpGet("List All Tours For Tour Operator/{userid}")]
        public async Task<IActionResult> listAllToursForTourOperator(int userid)
        {
            var tours = await context.Tours
            .Include(t => t.TourOperator) 
            .ThenInclude(to => to.User) 
            .Where(t => t.TourOperator.User.UserId == userid)
            .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();

            if (!tours.Any())
            {
                return NotFound("Not Found.");
            }

            return Ok(tours);
        }


        [HttpGet("List All Tours For Tour Operator Paging/{userid}")]
        public async Task<IActionResult> listAllToursForTourOperatorPaging(int userid, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalRecords = await context.Tours.CountAsync();

            var tours = await context.Tours.Include(t => t.TourOperator).ThenInclude(to => to.User)
            .Where(t => t.TourOperator.User.UserId == userid)
                .OrderBy(t => t.TourId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!tours.Any())
            {
                return NotFound("Not Found.");
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

        [HttpGet("Search Tour By Name For Tour Operator/{userid}")]
        public async Task<IActionResult> SearchTourForOperator(int userid, string? keyword)
        {
            var query = context.Tours.Include(t => t.TourOperator).ThenInclude(to => to.User)
            .Where(t => t.TourOperator.User.UserId == userid).AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u => u.Title.ToLower().Contains(keyword));
            }

            var tours = await query
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!tours.Any())
            {
                return NotFound("No tours found.");
            }

            return Ok(tours);
        }

        [HttpGet("Search Tour Paging By Name For Tour Operator/{userid}")]
        public async Task<IActionResult> SearchTourPagingForOperator(int userid, string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = context.Tours.Include(t => t.TourOperator).ThenInclude(to => to.User)
            .Where(t => t.TourOperator.User.UserId == userid).AsQueryable();

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

        [HttpGet("Filter Tours Paging For Tour Operator/{userid}")]
        public async Task<IActionResult> FilterToursPagingForOperator(int userid,
            string? title,
            string? transportation,
            string? startPoint,
            decimal? minPrice,
            decimal? maxPrice,
            [FromQuery] int[] ratings,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = context.Tours.Include(t => t.TourOperator).ThenInclude(to => to.User)
            .Where(t => t.TourOperator.User.UserId == userid).AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(t => t.Title.Contains(title));

            if (!string.IsNullOrEmpty(transportation))
                query = query.Where(t => t.Transportation!.Contains(transportation));

            if (!string.IsNullOrEmpty(startPoint))
                query = query.Where(t => t.StartPoint!.Contains(startPoint));

            if (minPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren <= maxPrice.Value);

            if (ratings != null && ratings.Length > 0)
            {
                query = query.Where(t =>
                    ratings.Contains(
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

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = result
            });
        }

        [HttpGet("Filter Tours For Operator/{userid}")]
        public async Task<IActionResult> FilterToursForOperattor(int userid,
            string? title,
            string? transportation,
            string? startPoint,
            decimal? minPrice,
            decimal? maxPrice,
            [FromQuery] int[] ratings)
        {
            var query = context.Tours.Include(t => t.TourOperator).ThenInclude(to => to.User)
            .Where(t => t.TourOperator.User.UserId == userid).AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(t => t.Title.Contains(title));

            if (!string.IsNullOrEmpty(transportation))
                query = query.Where(t => t.Transportation!.Contains(transportation));

            if (!string.IsNullOrEmpty(startPoint))
                query = query.Where(t => t.StartPoint!.Contains(startPoint));

            if (minPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren <= maxPrice.Value);

            if (ratings != null && ratings.Length > 0)
            {
                query = query.Where(t =>
                    ratings.Contains(
                        (int)Math.Round(
                            t.TourRatings.Any()
                            ? t.TourRatings.Average(r => (double?)r.Rating) ?? 0
                            : 0
                        )
                    )
                );
            }

            var result = await query
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(result);
        }

        /*[HttpGet("Tour Detail For TourOperator/{tourid}")]
        public async Task<IActionResult> TourDetailForTourOperator(int tourid)
        {
            var tour = await context.Tours
                .Include(x => x.TourOperator).ThenInclude(op => op.User)
                .Include(x => x.DepartureDates)
                .Include(x => x.TourExperiences)
                .Include(x => x.TourItineraries).ThenInclude(ti => ti.ItineraryMedia)
                .Include(x => x.TourMedia)
                .Include(x => x.TourRatings).ThenInclude(r => r.User)
                .FirstOrDefaultAsync(t => t.TourId == tourid);

            if (tour == null)
            {
                return NotFound("Tour not found.");
            }
            var result = _mapper.Map<TourDetailResponse>(tour);
            return Ok(result);
        }*/

        [HttpGet("Tour Detail For TourOperator/{tourid}")]
        public async Task<IActionResult> TourDetailForTourOperator(int tourid, bool forUpdate = false)
        {
            var tour = await context.Tours
                .Include(x => x.TourOperator).ThenInclude(op => op.User)
                .Include(x => x.DepartureDates)
                .Include(x => x.TourExperiences)
                .Include(x => x.TourItineraries).ThenInclude(ti => ti.ItineraryMedia)
                .Include(x => x.TourMedia)
                .Include(x => x.TourRatings).ThenInclude(r => r.User)
                .FirstOrDefaultAsync(t => t.TourId == tourid);

            if (tour == null)
            {
                return NotFound("Tour not found.");
            }

            // Nếu là forUpdate thì lọc DepartureDates
            if (forUpdate)
            {
                var today = DateTime.Today;
                tour.DepartureDates = tour.DepartureDates
                    .Where(d => d.DepartureDate1 > today)
                    .ToList();
            }

            var result = _mapper.Map<TourDetailResponse>(tour);
            return Ok(result);
        }

        [HttpGet("List All Tours For Customer")]
        public async Task<IActionResult> listAllToursForUser()
        {
            var tours = await context.Tours
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider).Where(t => t.IsActive == true)
                .ToListAsync();

            if (!tours.Any())
            {
                return NotFound("Not Found.");
            }

            return Ok(tours);
        }


        [HttpGet("List All Tours For Customer Paging")]
        public async Task<IActionResult> listAllToursForUserPaging(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalRecords = await context.Tours.CountAsync();

            var tours = await context.Tours
                .OrderBy(t => t.TourId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider).Where(t => t.IsActive == true)
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

        [HttpGet("Search Tour By Name For Customer")]
        public async Task<IActionResult> SearchTourForUser(string? keyword)
        {
            var query = context.Tours.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u => u.Title.ToLower().Contains(keyword));
            }

            var tours = await query
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!tours.Any())
            {
                return NotFound("No tours found.");
            }

            return Ok(tours);
        }

        [HttpGet("Search Tour Paging By Name For Customer")]
        public async Task<IActionResult> SearchTourPagingForUser(string? keyword, int pageNumber = 1, int pageSize = 10)
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

        [HttpGet("Filter Tours Paging For Customer")]
        public async Task<IActionResult> FilterToursPagingForCustomer(
            string? title,
            string? transportation,
            string? startPoint,
            decimal? minPrice,
            decimal? maxPrice,
            [FromQuery] int[] ratings,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = context.Tours.AsQueryable().Where(t => t.IsActive == true);

            if (!string.IsNullOrEmpty(title))
                query = query.Where(t => t.Title.Contains(title));

            if (!string.IsNullOrEmpty(transportation))
                query = query.Where(t => t.Transportation!.Contains(transportation));

            if (!string.IsNullOrEmpty(startPoint))
                query = query.Where(t => t.StartPoint!.Contains(startPoint));

            if (minPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren <= maxPrice.Value);

            if (ratings != null && ratings.Length > 0)
            {
                query = query.Where(t =>
                    ratings.Contains(
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

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = result
            });
        }

        [HttpGet("Filter Tours For Customer")]
        public async Task<IActionResult> FilterToursForCustomer(
            string? title,
            string? transportation,
            string? startPoint,
            decimal? minPrice,
            decimal? maxPrice,
            [FromQuery] int[] ratings)
        {
            var query = context.Tours.AsQueryable().Where(t => t.IsActive == true);

            if (!string.IsNullOrEmpty(title))
                query = query.Where(t => t.Title.Contains(title));

            if (!string.IsNullOrEmpty(transportation))
                query = query.Where(t => t.Transportation.Contains(transportation));

            if (!string.IsNullOrEmpty(startPoint))
                query = query.Where(t => t.StartPoint.Contains(startPoint));

            if (minPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(t => t.PriceOfAdults + t.PriceOfChildren <= maxPrice.Value);

            if (ratings != null && ratings.Length > 0)
            {
                query = query.Where(t =>
                    ratings.Contains(
                        (int)Math.Round(
                            t.TourRatings.Any()
                            ? t.TourRatings.Average(r => (double?)r.Rating) ?? 0
                            : 0
                        )
                    )
                );
            }

            var result = await query
                .ProjectTo<ListTourResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("Tour Detail For Customer/{tourid}")]
        public async Task<IActionResult> TourDetailForCustomer(int tourid)
        {
            var tour = await context.Tours
                .Include(x => x.TourOperator).ThenInclude(op => op.User)
                .Include(x => x.DepartureDates)
                .Include(x => x.TourExperiences)
                .Include(x => x.TourItineraries).ThenInclude(ti => ti.ItineraryMedia)
                .Include(x => x.TourMedia)
                .Include(x => x.TourRatings).ThenInclude(r => r.User)
                .Where(t => t.IsActive == true)
                .FirstOrDefaultAsync(t => t.TourId == tourid);

            if (tour == null)
            {
                return NotFound("Tour not found.");
            }

            var today = DateTime.UtcNow.AddHours(7).Date;
            tour.DepartureDates = tour.DepartureDates
                .Where(d => d.DepartureDate1.Date > today && d.IsActive)
                .ToList();

            var result = _mapper.Map<TourDetailResponse>(tour);
            return Ok(result);
        }


        [HttpPost("Create Tour")]
        public async Task<IActionResult> CreateTour([FromForm] TourCreateRequest request)
        {

            var slotInfo = await _slotCheckService.CheckRemainingSlotsAsync(request.TourOperatorId);

            /*if (slotInfo == null)
                return BadRequest("No remaining time create tour available. Please purchase a PackageService to create more Tours");*/

            if (slotInfo.MaxTour > 0) // 0 nghĩa là không giới hạn
            {
                var now = DateTime.UtcNow.AddHours(7);
                var currentMonthTourCount = await context.Tours
                    .Where(t => t.TourOperatorId == request.TourOperatorId &&
                        t.CreatedAt != null &&
                        t.CreatedAt.Value.Month == now.Month &&
                        t.CreatedAt.Value.Year == now.Year)
                    .CountAsync();

                if (currentMonthTourCount >= slotInfo.MaxTour)
                {
                    return BadRequest($"You have reached the monthly tour creation limit ({slotInfo.MaxTour}).");
                }
            }

            int totalImageCount = 0;

            var tour = new Tour
            {
                Title = request.Title,
                Description = request.Description,
                PriceOfAdults = request.PriceOfAdults,
                PriceOfChildren = request.PriceOfChildren,
                PriceOfInfants = request.PriceOfInfants,
                DurationInDays = request.DurationInDays,
                StartPoint = request.StartPoint,
                Transportation = request.Transportation,
                TourOperatorId = request.TourOperatorId,
                MaxSlots = request.MaxSlots,
                MinSlots = request.MinSlots,
                SlotsBooked = 0,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Note = request.Note,
                //TourAvartar = request.TourAvartar,
                TourStatus = "Active",
                IsActive = true,
            };

            if (request.TourAvartarFile == null || request.TourAvartarFile.Length == 0)
            {
                return BadRequest("Tour avatar is required.");
            }

            // Kiểm tra kích thước file (ví dụ: tối đa 10MB)
            if (request.TourAvartarFile.Length > 10 * 1024 * 1024)
            {
                return BadRequest("Tour avatar file size exceeds 10MB limit.");
            }

            // Kiểm tra loại file (chỉ cho phép ảnh)
            var contentTypeTourAvatar = request.TourAvartarFile.ContentType;
            if (!contentTypeTourAvatar.StartsWith("image/"))
            {
                return BadRequest("Only image files are allowed for tour avatar.");
            }

            // Upload lên Cloudinary
            var uploadParamsTourAvatar = new ImageUploadParams
            {
                File = new FileDescription(request.TourAvartarFile.FileName, request.TourAvartarFile.OpenReadStream()),
                Folder = "ProjectSEP490/Tour/TourAvartar",
                Transformation = new Transformation().Width(800).Height(800).Crop("fill") // Tối ưu ảnh
            };

            try
            {
                var uploadResult = await _cloudinary.UploadAsync(uploadParamsTourAvatar);
                tour.TourAvartar = uploadResult.SecureUrl.ToString();
                request.TourAvartarUrl = uploadResult.SecureUrl.ToString(); // Lưu URL vào request nếu cần
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading tour avatar: {ex.Message}");
            }

            // DepartureDates
            if (request.DepartureDates != null)
            {
                foreach (var d in request.DepartureDates)
                {

                    if (d.DepartureDate1.Date < DateTime.UtcNow.AddHours(7).Date)
                    {
                        return BadRequest($"Departure date {d.DepartureDate1:yyyy-MM-dd} cannot be earlier than today.");
                    }

                    tour.DepartureDates.Add(new DepartureDate
                    {
                        DepartureDate1 = d.DepartureDate1,
                        IsActive = true
                    });
                }
            }


            // TourExperiences
            if (request.TourExperiences != null)
            {
                foreach (var e in request.TourExperiences)
                {
                    tour.TourExperiences.Add(new TourExperience
                    {
                        Content = e.Content,
                        IsActive = true
                    });
                }
            }

            // TourItineraries
            if (request.TourItineraries != null)
            {
                foreach (var i in request.TourItineraries)
                {
                    var itinerary = new TourItinerary
                    {
                        DayNumber = i.DayNumber,
                        Title = i.Title,
                        Description = i.Description,
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        IsActive = true
                    };

                    // ItineraryMedia
                    if (i.ItineraryMedia != null)
                    {
                        totalImageCount += i.ItineraryMedia.Count(m => m.MediaFile?.ContentType.StartsWith("image/") == true);
                        foreach (var m in i.ItineraryMedia)
                        {
                            if (m.MediaFile == null || m.MediaFile.Length == 0)
                                return BadRequest("ItineraryMedia file is required.");

                            if (m.MediaFile.Length > 100 * 1024 * 1024)
                                return BadRequest("ItineraryMedia file size exceeds 100MB.");

                            // Kiểm tra loại thật sự của file dựa trên content-type
                            string contentType = m.MediaFile.ContentType;
                            bool isImage = contentType.StartsWith("image/");
                            bool isVideo = contentType.StartsWith("video/");
                            string declaredType = m.MediaType?.ToLower().Trim();

                            if (isImage && declaredType != "image")
                                return BadRequest("The uploaded file is an image, but MediaType was set to video.");

                            if (isVideo && declaredType != "video")
                                return BadRequest("The uploaded file is a video, but MediaType was set to image.");

                            if (!isImage && !isVideo)
                                return BadRequest("Unsupported file type. Only image and video are allowed.");

                            if (isVideo && !slotInfo.MaxVideo)
                                return BadRequest("Your service package does not allow uploading video in itinerary media.");

                            string uploadedUrl;

                            if (isVideo)
                            {
                                var uploadParams = new VideoUploadParams
                                {
                                    File = new FileDescription(m.MediaFile.FileName, m.MediaFile.OpenReadStream()),
                                    Folder = "ProjectSEP490/Tour/TourItineraryMedia/Video"
                                };
                                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                                uploadedUrl = uploadResult.SecureUrl.ToString();
                            }
                            else // Image
                            {
                                var uploadParams = new ImageUploadParams
                                {
                                    File = new FileDescription(m.MediaFile.FileName, m.MediaFile.OpenReadStream()),
                                    Folder = "ProjectSEP490/Tour/TourItineraryMedia/Image"
                                };
                                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                                uploadedUrl = uploadResult.SecureUrl.ToString();
                            }

                            itinerary.ItineraryMedia.Add(new ItineraryMedia
                            {
                                MediaUrl = uploadedUrl,
                                MediaType = isVideo ? "Video" : "Image",
                                Caption = m.Caption,
                                UploadedAt = DateTime.UtcNow.AddHours(7),
                                IsActive = true
                            });
                        }
                    }
                    tour.TourItineraries.Add(itinerary);
                }
            }

            // TourMedia
            if (request.TourMedia != null)
            {
                totalImageCount += request.TourMedia.Count(m => m.MediaFile?.ContentType.StartsWith("image/") == true);
                foreach (var m in request.TourMedia)
                {
                    if (m.MediaFile == null || m.MediaFile.Length == 0)
                        return BadRequest("TourMedia file is required.");

                    if (m.MediaFile.Length > 100 * 1024 * 1024)
                        return BadRequest("TourMedia file size exceeds 100MB.");

                    // Kiểm tra loại thật sự của file dựa trên content-type
                    string contentType = m.MediaFile.ContentType;
                    bool isImage = contentType.StartsWith("image/");
                    bool isVideo = contentType.StartsWith("video/");
                    string declaredType = m.MediaType?.ToLower().Trim();

                    if (isImage && declaredType != "image")
                        return BadRequest("The uploaded file is an image, but MediaType was set to video.");

                    if (isVideo && declaredType != "video")
                        return BadRequest("The uploaded file is a video, but MediaType was set to image.");

                    if (!isImage && !isVideo)
                        return BadRequest("Unsupported file type. Only image and video are allowed.");

                    if (isVideo && !slotInfo.MaxVideo)
                        return BadRequest("Your service package does not allow uploading video in tour media.");

                    string uploadedUrl;

                    if (isVideo)
                    {
                        var uploadParams = new VideoUploadParams
                        {
                            File = new FileDescription(m.MediaFile.FileName, m.MediaFile.OpenReadStream()),
                            Folder = "ProjectSEP490/Tour/TourMedia/Video"
                        };
                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        uploadedUrl = uploadResult.SecureUrl.ToString();
                    }
                    else // Image
                    {
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(m.MediaFile.FileName, m.MediaFile.OpenReadStream()),
                            Folder = "ProjectSEP490/Tour/TourMedia/Image"
                        };
                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        uploadedUrl = uploadResult.SecureUrl.ToString();
                    }

                    tour.TourMedia.Add(new TourMedia
                    {
                        MediaUrl = uploadedUrl,
                        MediaType = isVideo ? "Video" : "Image",
                        IsActive = true
                    });
                }
            }


            if (slotInfo.MaxImage > 0 && totalImageCount > slotInfo.MaxImage)
            {
                return BadRequest($"You can only upload up to {slotInfo.MaxImage} images per tour.");
            }

            context.Tours.Add(tour);

            try
            {
                await context.SaveChangesAsync();
                var activePackage = await context.PurchasedServicePackages
                        .Where(p => p.PurchaseId == slotInfo.PurchaseId)
                        .FirstOrDefaultAsync();

                if (activePackage != null)
                {
                    activePackage.NumOfToursUsed += 1;
                    await context.SaveChangesAsync();
                }


                return Ok(new { message = "Tour created successfully.", tourId = tour.TourId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("UpdateTour")]
        /*public async Task<IActionResult> UpdateTour([FromForm] TourUpdateRequest request)
        {
            var tour = context.Tours
                .Include(t => t.DepartureDates)
                .Include(t => t.TourExperiences)
                .Include(t => t.TourMedia)
                .Include(t => t.TourItineraries).ThenInclude(t => t.ItineraryMedia)
                .FirstOrDefault(u => u.TourId == request.TourId);
            if (tour == null)
            {
                return NotFound("Tour not found.");
            }

            tour.Title = request.Title;           
            tour.Description = request.Description;
            tour.PriceOfAdults = request.PriceOfAdults;
            tour.PriceOfChildren = request.PriceOfChildren;
            tour.PriceOfInfants = request.PriceOfInfants;
            tour.DurationInDays = request.DurationInDays;
            tour.StartPoint = request.StartPoint;
            tour.Transportation = request.Transportation;
            tour.MaxSlots = request.MaxSlots;
            tour.MinSlots = request.MinSlots;
            tour.Note = request.Note;
            tour.TourStatus = request.TourStatus;
            tour.IsActive = request.IsActive;

            // Update DepartureDates

            if (request.DepartureDates != null)
            {
                var today = DateTime.UtcNow.AddHours(7).Date;

                foreach (var d in request.DepartureDates)
                {
                    var existing = tour.DepartureDates.FirstOrDefault(x => x.Id == d.Id);
                    if (existing == null)
                    {
                        return BadRequest($"DepartureDate with ID {d.Id} not exist.");
                    }

                    if (existing.DepartureDate1.Date < today)
                    {
                        return BadRequest($"DepartureDate ID {d.Id} is in the past. You cannot update it.");
                    }


                    var hasActiveBooking = await context.Bookings
                        .AnyAsync(b => b.DepartureDateId == existing.Id && b.BookingStatus != "Cancelled");

                    if (hasActiveBooking)
                    {
                        return BadRequest($"DepartureDate ID {d.Id} already has active bookings. You cannot update it.");
                    }

                    if (d.DepartureDate1.Date <= today)
                    {
                        return BadRequest($"The new departure date of ID {d.Id} must be greater than today.");
                    }

                    var isDuplicated = tour.DepartureDates.Any(x => x.Id != d.Id
                                                                 && x.IsActive
                                                                 && x.DepartureDate1.Date == d.DepartureDate1.Date);
                    if (isDuplicated)
                    {
                        return BadRequest($"DepartureDate {d.DepartureDate1:yyyy-MM-dd} already exists in this tour.");
                    }

                    existing.DepartureDate1 = d.DepartureDate1;
                    existing.IsActive = d.IsActive;
                }
            }

            // Update TourExperiences
            if (request.TourExperiences != null)
            {
                foreach (var e in request.TourExperiences)
                {
                    var existing = tour.TourExperiences.FirstOrDefault(x => x.Id == e.Id);
                    if (existing != null)
                    {
                        existing.Content = e.Content;
                        existing.IsActive = e.IsActive;
                    }
                    else
                    {
                        return BadRequest($"TourExperience with ID {e.Id} not found.");
                    }
                }
            }

            // Update TourItineraries
            if (request.TourItineraries != null)
            {
                foreach (var i in request.TourItineraries)
                {
                    var existingItinerary = tour.TourItineraries
                        .FirstOrDefault(x => x.ItineraryId == i.ItineraryId);

                    if (existingItinerary != null)
                    {
                        existingItinerary.DayNumber = i.DayNumber;
                        existingItinerary.Title = i.Title;
                        existingItinerary.Description = i.Description;
                        existingItinerary.IsActive = i.IsActive;

                        // Update ItineraryMedia
                        if (i.ItineraryMedia != null)
                        {
                            foreach (var m in i.ItineraryMedia)
                            {
                                var existingMedia = existingItinerary.ItineraryMedia
                                    .FirstOrDefault(x => x.MediaId == m.MediaId);

                                if (existingMedia != null)
                                {
                                    // Nếu có file mới, kiểm tra & upload
                                    if (m.MediaFile != null && m.MediaFile.Length > 0)
                                    {
                                        if (m.MediaFile.Length > 100 * 1024 * 1024)
                                            return BadRequest("ItineraryMedia file size exceeds 100MB.");

                                        string contentType = m.MediaFile.ContentType;
                                        bool isImage = contentType.StartsWith("image/");
                                        bool isVideo = contentType.StartsWith("video/");
                                        string declaredType = m.MediaType?.ToLower().Trim();

                                        if (isImage && declaredType != "image")
                                            return BadRequest($"The uploaded file is an image, but MediaType was set to {declaredType}.");

                                        if (isVideo && declaredType != "video")
                                            return BadRequest($"The uploaded file is a video, but MediaType was set to {declaredType}.");

                                        if (!isImage && !isVideo)
                                            return BadRequest("Unsupported file type. Only image and video are allowed.");

                                        var tourOperatorId = existingItinerary.Tour.TourOperatorId;
                                        var slotInfo = await _slotCheckService.CheckRemainingSlotsAsync(tourOperatorId);
                                        if (slotInfo == null)
                                            return BadRequest("No active service package found.");

                                        if (isVideo && !slotInfo.PostVideo)
                                            return BadRequest("Current service plans do not allow uploading videos in media itinerary.");

                                        string uploadedUrl;

                                        if (isVideo)
                                        {
                                            var uploadParams = new VideoUploadParams
                                            {
                                                File = new FileDescription(m.MediaFile.FileName, m.MediaFile.OpenReadStream()),
                                                Folder = "ProjectSEP490/Tour/TourItineraryMedia/Video"
                                            };
                                            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                                            uploadedUrl = uploadResult.SecureUrl.ToString();
                                        }
                                        else
                                        {
                                            var uploadParams = new ImageUploadParams
                                            {
                                                File = new FileDescription(m.MediaFile.FileName, m.MediaFile.OpenReadStream()),
                                                Folder = "ProjectSEP490/Tour/TourItineraryMedia/Image"
                                            };
                                            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                                            uploadedUrl = uploadResult.SecureUrl.ToString();
                                        }

                                        existingMedia.MediaUrl = uploadedUrl;
                                        existingMedia.UploadedAt = DateTime.UtcNow.AddHours(7);
                                    }

                                    existingMedia.MediaType = m.MediaType;
                                    existingMedia.Caption = m.Caption;
                                    existingMedia.IsActive = m.IsActive;
                                }
                                else
                                {
                                    return BadRequest($"ItineraryMedia with ID {m.MediaId} not found.");
                                }
                            }
                        }
                    }
                    else
                    {
                        return BadRequest($"TourItinerary with ID {i.ItineraryId} not found.");
                    }
                }
            }

            // Update TourMedia
            if (request.TourMedia != null)
            {
                foreach (var m in request.TourMedia)
                {
                    var existing = tour.TourMedia.FirstOrDefault(x => x.Id == m.Id);
                    if (existing == null)
                        return BadRequest($"TourMedia with ID {m.Id} not found.");

                    if (m.MediaFile != null && m.MediaFile.Length > 0)
                    {
                        if (m.MediaFile.Length > 100 * 1024 * 1024)
                            return BadRequest("TourMedia file size exceeds 100MB.");

                        string contentType = m.MediaFile.ContentType;
                        bool isImage = contentType.StartsWith("image/");
                        bool isVideo = contentType.StartsWith("video/");
                        string declaredType = m.MediaType?.ToLower().Trim();

                        if (isImage && declaredType != "image")
                            return BadRequest($"The uploaded file is an image, but MediaType was set to {declaredType}.");

                        if (isVideo && declaredType != "video")
                            return BadRequest($"The uploaded file is a video, but MediaType was set to {declaredType}.");

                        if (!isImage && !isVideo)
                            return BadRequest("Unsupported file type. Only image and video are allowed.");

                        var tourOperatorId = tour.TourOperatorId;
                        var slotInfo = await _slotCheckService.CheckRemainingSlotsAsync(tourOperatorId);
                        if (slotInfo == null)
                            return BadRequest("No active service package found.");

                        if (isVideo && !slotInfo.PostVideo)
                            return BadRequest("Current service package does not allow uploading videos in tour media.");

                        string uploadedUrl;

                        if (isVideo)
                        {
                            var uploadParams = new VideoUploadParams
                            {
                                File = new FileDescription(m.MediaFile.FileName, m.MediaFile.OpenReadStream()),
                                Folder = "ProjectSEP490/Tour/TourMedia/Video"
                            };
                            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                            uploadedUrl = uploadResult.SecureUrl.ToString();
                        }
                        else
                        {
                            var uploadParams = new ImageUploadParams
                            {
                                File = new FileDescription(m.MediaFile.FileName, m.MediaFile.OpenReadStream()),
                                Folder = "ProjectSEP490/Tour/TourMedia/Image"
                            };
                            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                            uploadedUrl = uploadResult.SecureUrl.ToString();
                        }
                        existing.MediaUrl = uploadedUrl;
                    }

                    existing.MediaType = m.MediaType;
                    existing.IsActive = m.IsActive;
                }
            }



            await context.SaveChangesAsync();

            return Ok(new { message = "Tour updated successfully." });
        }*/

        public async Task<IActionResult> UpdateTour([FromForm] TourUpdateRequest request)
        {
            var tour = context.Tours
                .Include(t => t.DepartureDates)
                .Include(t => t.TourExperiences)
                .Include(t => t.TourMedia)
                .Include(t => t.TourItineraries).ThenInclude(t => t.ItineraryMedia)
                .FirstOrDefault(u => u.TourId == request.TourId);

            var slotInfo = await _slotCheckService.CheckRemainingSlotsAsync(tour.TourOperatorId);
            if (slotInfo == null)
                return BadRequest("No package found. Please purchase a service package to upload media.");


            if (tour == null)
            {
                return NotFound("Tour not found.");
            }

            tour.Title = request.Title;
            tour.Description = request.Description;
            tour.PriceOfAdults = request.PriceOfAdults;
            tour.PriceOfChildren = request.PriceOfChildren;
            tour.PriceOfInfants = request.PriceOfInfants;
            tour.DurationInDays = request.DurationInDays;
            tour.StartPoint = request.StartPoint;
            tour.Transportation = request.Transportation;
            tour.MaxSlots = request.MaxSlots;
            tour.MinSlots = request.MinSlots;
            tour.Note = request.Note;
            tour.TourStatus = request.TourStatus;
            tour.IsActive = request.IsActive;

            if (request.TourAvartarFile != null && request.TourAvartarFile.Length > 0)
            {
                // Kiểm tra kích thước file (ví dụ: tối đa 10MB)
                if (request.TourAvartarFile.Length > 10 * 1024 * 1024)
                {
                    return BadRequest("Tour avatar file size exceeds 10MB limit.");
                }

                // Kiểm tra loại file (chỉ cho phép ảnh)
                var contentTypeTourAvatar = request.TourAvartarFile.ContentType;
                if (!contentTypeTourAvatar.StartsWith("image/"))
                {
                    return BadRequest("Only image files are allowed for tour avatar.");
                }

                // Upload lên Cloudinary
                var uploadParamsTourAvatar = new ImageUploadParams
                {
                    File = new FileDescription(request.TourAvartarFile.FileName, request.TourAvartarFile.OpenReadStream()),
                    Folder = "ProjectSEP490/Tour/TourAvartar",
                    Transformation = new Transformation().Width(800).Height(800).Crop("fill")
                };

                try
                {
                    var uploadResult = await _cloudinary.UploadAsync(uploadParamsTourAvatar);
                    tour.TourAvartar = uploadResult.SecureUrl.ToString();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error uploading tour avatar: {ex.Message}");
                }
            }


            // Update DepartureDates

            if (request.DepartureDates != null)
            {
                var today = DateTime.UtcNow.AddHours(7).Date;
                //var slotInfo = await _slotCheckService.CheckRemainingSlotsAsync(tour.TourOperatorId);

                foreach (var d in request.DepartureDates)
                {
                    // Nếu có ID là 0 hoặc null => Tạo mới
                    if (d.Id == 0)
                    {
                        if (d.DepartureDate1.Date <= today)
                        {
                            return BadRequest("New departure date must be greater than today.");
                        }

                        if (tour.DepartureDates.Any(x => x.IsActive && x.DepartureDate1.Date == d.DepartureDate1.Date))
                        {
                            return BadRequest($"DepartureDate {d.DepartureDate1:yyyy-MM-dd} already exists in this tour.");
                        }

                        tour.DepartureDates.Add(new DepartureDate
                        {
                            DepartureDate1 = d.DepartureDate1,
                            IsActive = true
                        });
                    }
                    else
                    {
                        // Phần xử lý update DepartureDate như cũ
                        var existing = tour.DepartureDates.FirstOrDefault(x => x.Id == d.Id);
                        if (existing == null)
                        {
                            return BadRequest($"DepartureDate with ID {d.Id} not exist.");
                        }

                        if (existing.DepartureDate1.Date < today)
                        {
                            return BadRequest($"DepartureDate ID {d.Id} is in the past. You cannot update it.");
                        }

                        var hasActiveBooking = await context.Bookings
                            .AnyAsync(b => b.DepartureDateId == existing.Id && b.BookingStatus != "Cancelled");

                        if (hasActiveBooking)
                        {
                            return BadRequest($"DepartureDate ID {d.Id} already has active bookings. You cannot update it.");
                        }

                        if (d.DepartureDate1.Date <= today)
                        {
                            return BadRequest($"The new departure date of ID {d.Id} must be greater than today.");
                        }

                        var isDuplicated = tour.DepartureDates.Any(x => x.Id != d.Id
                                                                     && x.IsActive
                                                                     && x.DepartureDate1.Date == d.DepartureDate1.Date);
                        if (isDuplicated)
                        {
                            return BadRequest($"DepartureDate {d.DepartureDate1:yyyy-MM-dd} already exists in this tour.");
                        }

                        existing.DepartureDate1 = d.DepartureDate1;
                        existing.IsActive = d.IsActive;
                    }
                }
            }

            // Update TourExperiences
            if (request.TourExperiences != null)
            {
                //var slotInfo = await _slotCheckService.CheckRemainingSlotsAsync(tour.TourOperatorId);

                foreach (var e in request.TourExperiences)
                {
                    if (e.Id == 0) // Thêm mới
                    {
                        // Kiểm tra giới hạn package
                        int currentActive = tour.TourExperiences.Count(te => te.IsActive);

                        // Thêm mới
                        tour.TourExperiences.Add(new TourExperience
                        {
                            Content = e.Content,
                            IsActive = e.IsActive
                        });
                    }
                    else // Update existing
                    {
                        var existing = tour.TourExperiences.FirstOrDefault(x => x.Id == e.Id);
                        if (existing != null)
                        {
                            existing.Content = e.Content;
                            existing.IsActive = e.IsActive;
                        }
                        else
                        {
                            return BadRequest($"TourExperience with ID {e.Id} not found.");
                        }
                    }
                }
            }

            // Update TourItineraries
            if (request.TourItineraries != null)
            {
                foreach (var i in request.TourItineraries)
                {
                    if (i.ItineraryId == 0) // Thêm mới TourItinerary
                    {
                        // Kiểm tra giới hạn số ngày theo gói
                        int currentActive = tour.TourItineraries.Count(ti => ti.IsActive);

                        if (!int.TryParse(tour.DurationInDays, out var maxDays))
                            return BadRequest("DurationInDays của tour không hợp lệ.");

                        if (tour.TourItineraries.Count >= maxDays)
                            return BadRequest($"Tour đã đủ {maxDays} ngày lịch trình. Không thể thêm mới.");

                        var newItinerary = new TourItinerary
                        {
                            DayNumber = i.DayNumber,
                            Title = i.Title,
                            Description = i.Description,
                            CreatedAt = DateTime.UtcNow.AddHours(7),
                            IsActive = i.IsActive
                        };

                        // Thêm ItineraryMedia nếu có
                        if (i.ItineraryMedia != null)
                        {
                            foreach (var m in i.ItineraryMedia)
                            {
                                if (m.MediaId == 0 && m.MediaFile != null)
                                {
                                    // Kiểm tra giới hạn ảnh
                                    if (m.MediaType.ToLower() == "image")
                                    {
                                        int totalImages = tour.TourMedia.Count(x => x.IsActive && x.MediaType == "image") +
                                            tour.TourItineraries.SelectMany(ti => ti.ItineraryMedia).Count(im => im.IsActive && im.MediaType == "image");

                                        if (slotInfo.MaxImage > 0 && totalImages >= slotInfo.MaxImage)
                                            return BadRequest("Exceeded the image upload limit of the current package.");
                                    }

                                    // Upload media
                                    var uploadedUrl = await UploadMediaFile(m.MediaFile, m.MediaType, slotInfo);

                                    newItinerary.ItineraryMedia.Add(new ItineraryMedia
                                    {
                                        MediaUrl = uploadedUrl,
                                        MediaType = m.MediaType,
                                        Caption = m.Caption,
                                        UploadedAt = DateTime.UtcNow.AddHours(7),
                                        IsActive = m.IsActive
                                    });
                                }
                            }
                        }

                        tour.TourItineraries.Add(newItinerary);
                    }
                    else // Cập nhật TourItinerary đã có
                    {
                        var existingItinerary = tour.TourItineraries.FirstOrDefault(x => x.ItineraryId == i.ItineraryId);
                        if (existingItinerary == null)
                            return BadRequest($"TourItinerary với ID {i.ItineraryId} không tồn tại.");

                        existingItinerary.DayNumber = i.DayNumber;
                        existingItinerary.Title = i.Title;
                        existingItinerary.Description = i.Description;
                        existingItinerary.IsActive = i.IsActive;

                        // Update ItineraryMedia
                        if (i.ItineraryMedia != null)
                        {
                            foreach (var m in i.ItineraryMedia)
                            {
                                if (m.MediaId == 0 && m.MediaFile != null)
                                {
                                    // Kiểm tra giới hạn ảnh
                                    if (m.MediaType.ToLower() == "image")
                                    {
                                        int totalImages = tour.TourMedia.Count(x => x.IsActive && x.MediaType == "image") +
                                            tour.TourItineraries.SelectMany(ti => ti.ItineraryMedia).Count(im => im.IsActive && im.MediaType == "image");

                                        if (slotInfo.MaxImage > 0 && totalImages >= slotInfo.MaxImage)
                                            return BadRequest("Exceeded the image upload limit of the current package.");
                                    }

                                    var uploadedUrl = await UploadMediaFile(m.MediaFile, m.MediaType, slotInfo);

                                    existingItinerary.ItineraryMedia.Add(new ItineraryMedia
                                    {
                                        MediaUrl = uploadedUrl,
                                        MediaType = m.MediaType,
                                        Caption = m.Caption,
                                        UploadedAt = DateTime.UtcNow.AddHours(7),
                                        IsActive = m.IsActive
                                    });
                                }
                                else if (m.MediaId > 0)
                                {
                                    var existingMedia = existingItinerary.ItineraryMedia.FirstOrDefault(x => x.MediaId == m.MediaId);
                                    if (existingMedia == null)
                                        return BadRequest($"ItineraryMedia với ID {m.MediaId} không tồn tại.");

                                    if (m.MediaFile != null && m.MediaFile.Length > 0)
                                    {
                                        var uploadedUrl = await UploadMediaFile(m.MediaFile, m.MediaType, slotInfo);
                                        existingMedia.MediaUrl = uploadedUrl;
                                        existingMedia.UploadedAt = DateTime.UtcNow.AddHours(7);
                                    }

                                    existingMedia.MediaType = m.MediaType;
                                    existingMedia.Caption = m.Caption;
                                    existingMedia.IsActive = m.IsActive;
                                }
                            }
                        }
                    }
                }
            }

            // Update TourMedia
            if (request.TourMedia != null)
            {
                foreach (var m in request.TourMedia)
                {
                    if (m.Id == 0) // Thêm mới
                    {
                        int currentImages = tour.TourMedia.Count(x => x.IsActive && x.MediaType.Equals("image", StringComparison.OrdinalIgnoreCase));
                        int currentVideos = tour.TourMedia.Count(x => x.IsActive && x.MediaType.Equals("video", StringComparison.OrdinalIgnoreCase));

                        if (m.MediaType.Equals("image", StringComparison.OrdinalIgnoreCase) &&
                            slotInfo.MaxImage > 0 && currentImages >= slotInfo.MaxImage)
                        {
                            return BadRequest("Exceeded the image upload limit of the current package.");
                        }

                        if (m.MediaType.Equals("video", StringComparison.OrdinalIgnoreCase) &&
                            !slotInfo.MaxVideo)
                        {
                            return BadRequest("Current service package does not allow uploading videos.");
                        }

                        if (m.MediaFile == null || m.MediaFile.Length == 0)
                            return BadRequest("Media file is required for new TourMedia.");

                        try
                        {
                            var uploadedUrl = await UploadMediaFile(m.MediaFile, m.MediaType, slotInfo);

                            tour.TourMedia.Add(new TourMedia
                            {
                                MediaUrl = uploadedUrl,
                                MediaType = m.MediaType,
                                IsActive = m.IsActive
                            });
                        }
                        catch (ArgumentException ex)
                        {
                            return BadRequest(ex.Message);
                        }
                    }
                    else // Update existing
                    {
                        var existing = tour.TourMedia.FirstOrDefault(x => x.Id == m.Id);
                        if (existing == null)
                            return BadRequest($"TourMedia with ID {m.Id} not found.");

                        if (m.MediaFile != null && m.MediaFile.Length > 0)
                        {
                            try
                            {
                                var uploadedUrl = await UploadMediaFile(m.MediaFile, m.MediaType, slotInfo);
                                existing.MediaUrl = uploadedUrl;
                            }
                            catch (ArgumentException ex)
                            {
                                return BadRequest(ex.Message);
                            }
                        }

                        existing.MediaType = m.MediaType;
                        existing.IsActive = m.IsActive;
                    }
                }
            }
            await context.SaveChangesAsync();

            return Ok(new { message = "Tour updated successfully." });
        }


        [HttpPatch("ToggleTourStatus/{tourid}")]
        public async Task<IActionResult> ToggleTourStatus(int tourid)
        {
            var tour = await context.Tours.FindAsync(tourid);
            if (tour == null)
            {
                return NotFound("Tour not found.");
            }

            // Toggle trạng thái IsActive
            tour.IsActive = !tour.IsActive;

            // Cập nhật TourStatus tương ứng
            tour.TourStatus = tour.IsActive ? "Active" : "InActive";

            await context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Tour has been {(tour.IsActive ? "activated" : "deactivated")}",
                tourId = tourid,
                newStatus = tour.IsActive,
                tourStatus = tour.TourStatus
            });
        }

        private async Task<string> UploadMediaFile(IFormFile mediaFile, string mediaType, CheckSlotTourOperatorResponse slotInfo)
        {
            if (mediaFile == null || mediaFile.Length == 0)
                throw new ArgumentException("Media file is required.");

            if (mediaFile.Length > 100 * 1024 * 1024)
                throw new ArgumentException("Media file size exceeds 100MB.");

            string contentType = mediaFile.ContentType;
            bool isImage = contentType.StartsWith("image/");
            bool isVideo = contentType.StartsWith("video/");
            string declaredType = mediaType?.ToLower().Trim();

            if (isImage && declaredType != "image")
                throw new ArgumentException("The uploaded file is an image, but MediaType was set to video.");

            if (isVideo && declaredType != "video")
                throw new ArgumentException("The uploaded file is a video, but MediaType was set to image.");

            if (!isImage && !isVideo)
                throw new ArgumentException("Unsupported file type. Only image and video are allowed.");

            if (isVideo && !slotInfo.MaxVideo)
                throw new ArgumentException("Current service package does not allow uploading videos.");

            string uploadedUrl;
            string folder = isVideo ? "ProjectSEP490/Tour/TourItineraryMedia/Video"
                                   : "ProjectSEP490/Tour/TourItineraryMedia/Image";

            if (isVideo)
            {
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(mediaFile.FileName, mediaFile.OpenReadStream()),
                    Folder = folder
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                uploadedUrl = uploadResult.SecureUrl.ToString();
            }
            else
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(mediaFile.FileName, mediaFile.OpenReadStream()),
                    Folder = folder
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                uploadedUrl = uploadResult.SecureUrl.ToString();
            }

            return uploadedUrl;
        }
    }
}
