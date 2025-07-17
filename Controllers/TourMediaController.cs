using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data;
using TourManagement_BE.Data.DTO.Request.TourMediaRequest;
using TourManagement_BE.Models;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourMediaController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;

        public TourMediaController(MyDBContext context, IMapper mapper, Cloudinary cloudinary)
        {
            this.context = context;
            _mapper = mapper;
            this._cloudinary = cloudinary;
        }

        [HttpPost("CreateTourMedia")]
        public async Task<IActionResult> CreateTourMedia([FromForm] TourMediaCreateRequest request)
        {
            var tour = await context.Tours.FindAsync(request.TourId);
            if (tour == null)
                return NotFound("Tour not found.");

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
                    Folder = "ProjectSEP490/Tour/TourMedia/Video"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                uploadedUrl = uploadResult.SecureUrl.ToString();
            }
            else
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(request.MediaFile.FileName, request.MediaFile.OpenReadStream()),
                    Folder = "ProjectSEP490/Tour/TourMedia/Image"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                uploadedUrl = uploadResult.SecureUrl.ToString();
            }

            var media = new TourMedia
            {
                MediaUrl = uploadedUrl,
                MediaType = request.MediaType,
                IsActive = true
            };

            tour.TourMedia.Add(media);
            await context.SaveChangesAsync();

            return Ok(new { message = "TourMedia created successfully.", id = media.Id });
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
    }
}
