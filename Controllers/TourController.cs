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
using TourManagement_BE.Service.TourManagement;

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
        private readonly ITourService _tourService;
        

        public TourController(MyDBContext context, IMapper mapper, Cloudinary cloudinary, ISlotCheckService slotCheckService, ITourService tourService)
        {
            this.context = context;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _slotCheckService = slotCheckService;
            _tourService = tourService;
        }


        [HttpGet("touroperator/{touroperatorid}/tours")]
        public async Task<IActionResult> TourOperatorFullTourListPaging(int touroperatorid, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (touroperatorid <= 0)
                    return BadRequest("Invalid tour operator ID.");

                if (pageNumber <= 0 || pageSize <= 0)
                    return BadRequest("Page number and page size must be greater than 0.");

                var result = await _tourService.TourOperatorFullTourListPagingAsync(touroperatorid, pageNumber, pageSize);

                if (result == null || !result.Data.Any())
                {
                    return NotFound("No tours found for this tour operator.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("touroperator/{touroperatorid}/tours/search")]
        public async Task<IActionResult> SearchPagingTourOperatorFullTourList(int touroperatorid, string keyword, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (touroperatorid <= 0)
                    return BadRequest("Invalid tour operator ID.");

                if (string.IsNullOrWhiteSpace(keyword))
                    return BadRequest("Search keyword cannot be empty.");

                if (pageNumber <= 0 || pageSize <= 0)
                    return BadRequest("Page number and page size must be greater than 0.");

                var result = await _tourService.SearchPagingTourOperatorFullTourListAsync(touroperatorid, keyword, pageNumber, pageSize);

                if (result == null || !result.Data.Any())
                {
                    return NotFound("No tours found matching the search criteria.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("touroperator/{touroperatorid}/tours/filter")]
        public async Task<IActionResult> FilterPagingTourOperatorFullTourList(int touroperatorid,
            string? title,
            string? transportation,
            string? startPoint,
            decimal? minPrice,
            decimal? maxPrice,
            [FromQuery] int[]? ratings,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                if (touroperatorid <= 0)
                    return BadRequest("Invalid tour operator ID.");

                var filter = new TourFilterRequest
                {
                    Title = title,
                    Transportation = transportation,
                    StartPoint = startPoint,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    Ratings = ratings ?? Array.Empty<int>()
                };

                var result = await _tourService.FilterPagingTourOperatorFullTourListAsync(touroperatorid, filter, pageNumber, pageSize);

                if (result == null || !result.Data.Any())
                {
                    return NotFound("No tours found matching the filter criteria.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("List All Tours For Tour Operator/{userid}")]
        public async Task<IActionResult> listAllToursForTourOperator(int userid)
        {
            var tours = await _tourService.ListAllForTourOperatorAsync(userid);

            if (!tours.Any())
            {
                return NotFound("Not Found.");
            }

            return Ok(tours);
        }


        [HttpGet("List All Tours For Tour Operator Paging/{userid}")]
        public async Task<IActionResult> listAllToursForTourOperatorPaging(int userid, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _tourService.ListAllForTourOperatorPagingAsync(userid, pageNumber, pageSize);
            if (result == null || !result.Data.Any())
            {
                return NotFound("Not Found.");
            }
            return Ok(result);
        }

        [HttpGet("Search Tour By Name For Tour Operator/{userid}")]
        public async Task<IActionResult> SearchTourForOperator(int userid, string? keyword)
        {
            var tours = await _tourService.SearchForOperatorAsync(userid, keyword);
            if (tours == null || !tours.Any())
            {
                return NotFound("No tours found.");
            }
            return Ok(tours);
        }

        [HttpGet("Search Tour Paging By Name For Tour Operator/{userid}")]
        public async Task<IActionResult> SearchTourPagingForOperator(int userid, string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _tourService.SearchPagingForOperatorAsync(userid, keyword, pageNumber, pageSize);
            if (result == null || !result.Data.Any())
            {
                return NotFound("No tours found.");
            }
            return Ok(result);
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
            var filter = new TourFilterRequest
            {
                Title = title,
                Transportation = transportation,
                StartPoint = startPoint,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Ratings = ratings
            };

            var result = await _tourService.FilterForOperatorPagingAsync(userid, filter, pageNumber, pageSize);
            return Ok(result);
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
            var filter = new TourFilterRequest
            {
                Title = title,
                Transportation = transportation,
                StartPoint = startPoint,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Ratings = ratings
            };

            var tours = await _tourService.FilterForOperatorAsync(userid, filter);
            return Ok(tours); 
        }

        [HttpGet("Tour Detail For TourOperator/{tourid}")]
        public async Task<IActionResult> TourDetailForTourOperator(int tourid, bool forUpdate = false)
        {
            var tour = await _tourService.GetDetailForOperatorAsync(tourid, forUpdate);
            if (tour == null)
            {
                return NotFound("Tour not found.");
            }
            return Ok(tour);
        }

        [HttpGet("List All Tours For Customer")]
        public async Task<IActionResult> listAllToursForUser()
        {
            var tours = await _tourService.ListAllForCustomerAsync();
            if (tours == null || !tours.Any())
            {
                return NotFound("Not Found.");
            }
            return Ok(tours);
        }


        [HttpGet("List All Tours For Customer Paging")]
        public async Task<IActionResult> listAllToursForUserPaging(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _tourService.ListAllForCustomerPagingAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("Search Tour By Name For Customer")]
        public async Task<IActionResult> SearchTourForUser(string? keyword)
        {
            var tours = await _tourService.SearchForCustomerAsync(keyword);
            if (tours == null || !tours.Any())
            {
                return NotFound("No tours found.");
            }
            return Ok(tours);
        }

        [HttpGet("Search Tour Paging By Name For Customer")]
        public async Task<IActionResult> SearchTourPagingForUser(string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _tourService.SearchPagingForCustomerAsync(keyword, pageNumber, pageSize);
            if (result == null || !result.Data.Any())
            {
                return NotFound("No tours found.");
            }
            return Ok(result);
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
            var filter = new TourFilterRequest
            {
                Title = title,
                Transportation = transportation,
                StartPoint = startPoint,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Ratings = ratings
            };

            var result = await _tourService.FilterForCustomerPagingAsync(filter, pageNumber, pageSize);
            return Ok(result);
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
            var filter = new TourFilterRequest
            {
                Title = title,
                Transportation = transportation,
                StartPoint = startPoint,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Ratings = ratings
            };

            var tours = await _tourService.FilterForCustomerAsync(filter);
            return Ok(tours);
        }

        [HttpGet("Tour Detail For Customer/{tourid}")]
        public async Task<IActionResult> TourDetailForCustomer(int tourid)
        {
            var tour = await _tourService.GetDetailForCustomerAsync(tourid);
            if (tour == null)
            {
                return NotFound("Tour not found.");
            }
            return Ok(tour);
        }


        [HttpPost("Create Tour")]
        
        public async Task<IActionResult> CreateTour([FromForm] TourCreateRequest request)
        {

            var slotInfo = await _slotCheckService.CheckRemainingSlotsAsync(request.TourOperatorId);

            /*if (slotInfo == null)
                return BadRequest("No remaining time create tour available. Please purchase a PackageService to create more Tours");*/

            /*if (slotInfo.MaxTour > 0) // 0 nghĩa là không giới hạn
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
            }*/

            var now = DateTime.UtcNow.AddHours(7);

            // 1️⃣ Nếu là gói miễn phí (PurchaseId = 0) → đếm số tour đã tạo trong tháng
            if (slotInfo.PurchaseId == 0)
            {
                var currentMonthTourCount = await context.Tours
                    .Where(t => t.TourOperatorId == request.TourOperatorId &&
                                t.CreatedAt != null &&
                                t.CreatedAt.Value.Month == now.Month &&
                                t.CreatedAt.Value.Year == now.Year &&
                                t.IsActive)
                    .CountAsync();

                if (slotInfo.MaxTour > 0 && currentMonthTourCount >= slotInfo.MaxTour)
                {
                    return BadRequest($"Bạn đã đạt giới hạn tạo tour trong tháng ({slotInfo.MaxTour}) cho gói miễn phí.");
                }
            }
            // 2️⃣ Nếu là gói trả phí → dùng NumOfToursUsed trong PurchasedServicePackages
            else
            {
                if (slotInfo.MaxTour > 0 && slotInfo.NumOfToursUsed >= slotInfo.MaxTour)
                {
                    return BadRequest($"Bạn đã đạt giới hạn tạo tour trong tháng ({slotInfo.MaxTour}) cho gói hiện tại.");
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
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading tour avatar: {ex.Message}");
            }

            // DepartureDates
            

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
    //tour.DurationInDays = request.DurationInDays;
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
            return BadRequest("Kích thước tệp ảnh đại diện của tour du lịch vượt quá giới hạn 10MB.");
        }

        // Kiểm tra loại file (chỉ cho phép ảnh)
        var contentTypeTourAvatar = request.TourAvartarFile.ContentType;
        if (!contentTypeTourAvatar.StartsWith("image/"))
        {
            return BadRequest("Chỉ được phép sử dụng tệp hình ảnh làm ảnh đại diện cho tour du lịch.");
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
            return StatusCode(500, $"Lỗi khi tải ảnh đại diện của tour: {ex.Message}");
        }
    }

    if (request.TourExperiences != null)
    {
        var existingExperiences = tour.TourExperiences.ToList();

        // Xóa những cái không còn trong request
        var requestIds = request.TourExperiences.Where(e => e.Id > 0).Select(e => e.Id).ToList();
        var toDelete = existingExperiences.Where(e => !requestIds.Contains(e.Id)).ToList();

        foreach (var del in toDelete)
        {
            context.TourExperiences.Remove(del);
        }

        // Thêm mới hoặc update
        foreach (var e in request.TourExperiences)
        {
            if (e.Id == 0) // Thêm mới
            {
                tour.TourExperiences.Add(new TourExperience
                {
                    Content = e.Content,
                    IsActive = e.IsActive
                });
            }
            else // Update
            {
                var existing = existingExperiences.FirstOrDefault(x => x.Id == e.Id);
                if (existing != null)
                {
                    existing.Content = e.Content;
                    existing.IsActive = e.IsActive;
                }
            }
        }
    }

    // UpdateItineraries: xoá hết cũ, thêm lại mới
    if (request.TourItineraries != null)
    {
        // Xoá toàn bộ ItineraryMedia và TourItineraries cũ
        foreach (var oldItinerary in tour.TourItineraries.ToList())
        {
            if (oldItinerary.ItineraryMedia != null && oldItinerary.ItineraryMedia.Any())
            {
                context.ItineraryMedia.RemoveRange(oldItinerary.ItineraryMedia);
            }
            context.TourItineraries.Remove(oldItinerary);
        }

        // Thêm mới hoàn toàn từ request
        foreach (var i in request.TourItineraries)
        {
            var newItinerary = new TourItinerary
            {
                DayNumber = i.DayNumber,
                Title = i.Title,
                Description = i.Description,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                IsActive = i.IsActive,
                ItineraryMedia = new List<ItineraryMedia>()
            };

            if (i.ItineraryMedia != null)
            {
                foreach (var m in i.ItineraryMedia)
                {
                    if (m.MediaFile != null && m.MediaFile.Length > 0)
                    {
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

        // ✅ Dùng request để set DurationInDays
        tour.DurationInDays = request.TourItineraries.Count.ToString();
    }

    // Update TourMedia
    if (request.TourMedia != null)
    {

        var existingMediaList = tour.TourMedia.ToList();
        var requestIds = request.TourMedia.Where(m => m.Id > 0).Select(m => m.Id).ToList();

        // Xóa những media không còn trong request
        var toDelete = existingMediaList.Where(m => !requestIds.Contains(m.Id)).ToList();
        foreach (var del in toDelete)
        {
            context.TourMedia.Remove(del);
        }

        foreach (var m in request.TourMedia)
        {
            if (m.Id == 0) // Thêm mới
            {
                int currentImages = tour.TourMedia.Count(x => x.IsActive && x.MediaType.Equals("image", StringComparison.OrdinalIgnoreCase));
                int currentVideos = tour.TourMedia.Count(x => x.IsActive && x.MediaType.Equals("video", StringComparison.OrdinalIgnoreCase));

                if (m.MediaType.Equals("image", StringComparison.OrdinalIgnoreCase) &&
                    slotInfo.MaxImage > 0 && currentImages >= slotInfo.MaxImage)
                {
                    return BadRequest("Vượt quá giới hạn tải lên hình ảnh của gói hiện tại.");
                }

                if (m.MediaType.Equals("video", StringComparison.OrdinalIgnoreCase) &&
                    !slotInfo.MaxVideo)
                {
                    return BadRequest("Gói dịch vụ hiện tại không cho phép tải video lên.");
                }

                if (m.MediaFile == null || m.MediaFile.Length == 0)
                    return BadRequest("Tệp phương tiện là bắt buộc đối với TourMedia mới.");

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
                    return BadRequest($"TourMedia với ID {m.Id} không tìm thấy.");

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

    return Ok(new { message = "Tour đã được cập nhật thành công." });
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
