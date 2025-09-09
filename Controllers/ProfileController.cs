using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.ProfileRequest;
using TourManagement_BE.Data.DTO.Response.ProfileResponse;
using TourManagement_BE.Service.Profile;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : Controller
    {
        private readonly MyDBContext context;
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;
        private readonly IProfileService _profileService;
        public ProfileController(MyDBContext context, CloudinaryDotNet.Cloudinary cloudinary, IProfileService profileService)
        {
            this.context = context;
            _cloudinary = cloudinary;
            _profileService = profileService;
        }

        [HttpGet("ViewProfile/{userId}")]
        public async Task<IActionResult> ViewProfile(int userId)
        {
            var userProfile = await _profileService.GetUserProfileAsync(userId);
            if (userProfile == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }
            return Ok(userProfile);
        }

        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _profileService.UpdateUserProfileAsync(request);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new
            {
                message = result.Message,
                user = result.UserProfile
            });
        }

    }
}
