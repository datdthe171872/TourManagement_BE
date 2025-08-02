using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.TourItineraryRequest;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Repository.Interface;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourItineraryMediaController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly ISlotCheckService _slotCheckService;
        public TourItineraryMediaController(MyDBContext context, IMapper mapper, Cloudinary cloudinary, ISlotCheckService slotCheckService)
        {
            this.context = context;
            _mapper = mapper;
            this._cloudinary = cloudinary;
            _slotCheckService = slotCheckService;
        }

        [HttpPost("AddItineraryMedia")]
        public async Task<IActionResult> AddItineraryMedia([FromForm] ItineraryMediaCreate request)
        {
            var itinerary = await context.TourItineraries
                .Include(i => i.Tour)
                .Include(i => i.ItineraryMedia)
                .FirstOrDefaultAsync(i => i.ItineraryId == request.ItineraryId);

            if (itinerary == null)
                return NotFound("Itinerary not found.");

            var slotInfo = await _slotCheckService.CheckRemainingSlotsAsync(itinerary.Tour.TourOperatorId);
            if (slotInfo == null)
                return BadRequest("No active service package found.");

            var m = request;

            if (m.MediaFile == null || m.MediaFile.Length == 0)
                return BadRequest("ItineraryMedia file is required.");

            if (m.MediaFile.Length > 100 * 1024 * 1024)
                return BadRequest("ItineraryMedia file size exceeds 100MB.");

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

            // ✅ Check MaxVideo
            if (isVideo && !slotInfo.MaxVideo)
                return BadRequest("Gói dịch vụ hiện tại không cho phép upload video trong itinerary media.");

            // ✅ Check MaxImage
            if (isImage && slotInfo.MaxImage > 0)
            {
                int currentImageCount = itinerary.ItineraryMedia
                    .Count(x => x.IsActive && x.MediaType.ToLower() == "image");

                if (currentImageCount >= slotInfo.MaxImage)
                    return BadRequest($"Gói hiện tại chỉ cho phép tối đa {slotInfo.MaxImage} ảnh cho mỗi lịch trình.");
            }

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

            itinerary.ItineraryMedia.Add(new ItineraryMedia
            {
                MediaUrl = uploadedUrl,
                MediaType = isVideo ? "Video" : "Image",
                Caption = m.Caption,
                UploadedAt = DateTime.UtcNow.AddHours(7),
                IsActive = true
            });

            await context.SaveChangesAsync();

            return Ok(new { message = "Itinerary media created successfully." });
        }


        [HttpDelete("SoftDeleteTourItineraryMedia/{id}")]
        public async Task<IActionResult> SoftDeleteTourItineraryMedia(int id)
        {
            var media = await context.ItineraryMedia.FindAsync(id);
            if (media == null)
                return NotFound("ItineraryMedia not found.");

            media.IsActive = false;
            await context.SaveChangesAsync();

            return Ok(new { message = "ItineraryMedia deactivated successfully." });
        }

        [HttpPatch("ToggleTourItineraryMedia/{id}")]
        public async Task<IActionResult> ToggleTourItineraryMedia(int id)
        {
            var media = await context.ItineraryMedia.FindAsync(id);
            if (media == null)
                return NotFound("ItineraryMedia not found.");

            media.IsActive = !media.IsActive;

            await context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Tour Itinerary Media has been {(media.IsActive ? "activated" : "deactivated")}",
            });
        }
    }
}
