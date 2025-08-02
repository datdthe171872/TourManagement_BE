using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.TourMediaRequest;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Repository.Interface;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourMediaController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly ISlotCheckService _slotCheckService;
        public TourMediaController(MyDBContext context, IMapper mapper, Cloudinary cloudinary, ISlotCheckService slotCheckService)
        {
            this.context = context;
            _mapper = mapper;
            this._cloudinary = cloudinary;
            _slotCheckService = slotCheckService;
        }


        [HttpPost("CreateTourMedia")]
        public async Task<IActionResult> CreateTourMedia([FromForm] TourMediaCreateRequest request)
        {
            var tour = await context.Tours
                .Include(t => t.TourMedia)
                .FirstOrDefaultAsync(t => t.TourId == request.TourId);

            if (tour == null)
                return NotFound("Tour not found.");

            var slotInfo = await _slotCheckService.CheckRemainingSlotsAsync(tour.TourOperatorId);
            if (slotInfo == null)
                return BadRequest("No active service package found.");

            var m = request;

            if (m.MediaFile == null || m.MediaFile.Length == 0)
                return BadRequest("Media file is required.");

            if (m.MediaFile.Length > 100 * 1024 * 1024)
                return BadRequest("Media file exceeds 100MB.");

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
                return BadRequest("Gói dịch vụ hiện tại không cho phép upload video trong tour media.");

            if (isImage && slotInfo.MaxImage > 0)
            {
                int currentImageCount = tour.TourMedia
                    .Count(x => x.IsActive && x.MediaType.ToLower() == "image");

                if (currentImageCount >= slotInfo.MaxImage)
                    return BadRequest($"Gói hiện tại chỉ cho phép tối đa {slotInfo.MaxImage} ảnh cho mỗi tour.");
            }

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

            tour.TourMedia.Add(new TourMedia
            {
                MediaUrl = uploadedUrl,
                MediaType = request.MediaType,
                IsActive = true
            });

            await context.SaveChangesAsync();

            return Ok(new { message = "Tour media created successfully." });
        }



        [HttpDelete("SoftDeleteTourMedia/{id}")]
        public async Task<IActionResult> SoftDeleteTourMedia(int id)
        {
            var media = await context.TourMedia.FindAsync(id);
            if (media == null)
                return NotFound("TourMedia not found.");

            media.IsActive = false;
            await context.SaveChangesAsync();

            return Ok(new { message = "TourMedia deactivated successfully." });
        }

        [HttpPatch("ToggleTourMedia/{id}")]
        public async Task<IActionResult> ToggleTourMedia(int id)
        {
            var media = await context.TourMedia.FindAsync(id);
            if (media == null)
                return NotFound("TourMedia not found.");

            media.IsActive = !media.IsActive;

            await context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Tour Media has been {(media.IsActive ? "activated" : "deactivated")}",
            });
        }
    }
}
