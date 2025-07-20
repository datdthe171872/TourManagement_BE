using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.TourItineraryRequest;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourItineraryMediaController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;

        public TourItineraryMediaController(MyDBContext context, IMapper mapper, Cloudinary cloudinary)
        {
            this.context = context;
            _mapper = mapper;
            this._cloudinary = cloudinary;
        }

        [HttpPost("AddItineraryMedia")]
        public async Task<IActionResult> AddItineraryMedia([FromForm] ItineraryMediaCreate request)
        {
            var itinerary = await context.TourItineraries.FindAsync(request.ItineraryId);
            if (itinerary == null)
                return NotFound("Itinerary not found.");

            if (request.MediaFile == null || request.MediaFile.Length == 0)
                return BadRequest("Media file is required.");

            if (request.MediaFile.Length > 100 * 1024 * 1024)
                return BadRequest("Media file exceeds 100MB.");

            string uploadedUrl;

            if (request.MediaType == "Video")
            {
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(request.MediaFile.FileName, request.MediaFile.OpenReadStream()),
                    Folder = "ProjectSEP490/Tour/TourItineraryMedia/Video"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                uploadedUrl = uploadResult.SecureUrl.ToString();
            }
            else
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(request.MediaFile.FileName, request.MediaFile.OpenReadStream()),
                    Folder = "ProjectSEP490/Tour/TourItineraryMedia/Image"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                uploadedUrl = uploadResult.SecureUrl.ToString();
            }

            var media = new ItineraryMedia
            {
                MediaUrl = uploadedUrl,
                MediaType = request.MediaType,
                Caption = request.Caption,
                UploadedAt = DateTime.UtcNow.AddHours(7),
                IsActive = true
            };

            itinerary.ItineraryMedia.Add(media);
            await context.SaveChangesAsync();

            return Ok(new { message = "Itinerary media created successfully.", mediaId = media.MediaId });
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
    }
}
