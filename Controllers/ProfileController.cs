using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using TourManagement_BE.Data;
using TourManagement_BE.Data.DTO.Request.ProfileRequest;
using TourManagement_BE.Data.DTO.Response.ProfileResponse;
using System.ComponentModel.DataAnnotations;
using TourManagement_BE.Models;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : Controller
    {
        private readonly MyDBContext context;
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;
        public ProfileController(MyDBContext context, CloudinaryDotNet.Cloudinary cloudinary)
        {
            this.context = context;
            _cloudinary = cloudinary;
        }

        [HttpGet("ViewProfile/{userId}")]
        public IActionResult ViewProfile(int userId)
        {
            var user = context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new UserProfileResponse
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    Address = u.Address,
                    PhoneNumber = u.PhoneNumber,
                    Avatar = u.Avatar,
                    RoleName = u.Role.RoleName,
                    IsActive = u.IsActive
                })
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var tourOperator = context.TourOperators.FirstOrDefault(t => t.UserId == userId);
            if (tourOperator != null)
            {
                user.CompanyName = tourOperator.CompanyName;
                user.Description = tourOperator.Description;
            }

            var tourGuide = context.TourGuides.FirstOrDefault(t => t.UserId == userId);
            if (tourGuide != null)
            {
                user.HomeTourGuideId = tourGuide.TourOperatorId;
            }

            return Ok(user);
        }

        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = context.Users.FirstOrDefault(u => u.UserId == request.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!string.IsNullOrWhiteSpace(request.UserName))
            {
                user.UserName = request.UserName;
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailAttribute = new EmailAddressAttribute();
                if (!emailAttribute.IsValid(request.Email))
                {
                    return BadRequest("Email không hợp lệ.");
                }

                var emailExists = context.Users.Any(x => x.Email == request.Email && x.UserId != request.UserId);
                if (emailExists)
                {
                    return BadRequest("Email đã được sử dụng bởi người dùng khác.");
                }

                user.Email = request.Email;
            }

            if (!string.IsNullOrWhiteSpace(request.Address))
            {
                user.Address = request.Address;
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                user.PhoneNumber = request.PhoneNumber;
            }

            if (request.AvatarFile != null && request.AvatarFile.Length > 0)
            {
                if (request.AvatarFile.Length > 5 * 1024 * 1024)
                    return BadRequest("Kích thước ảnh tối đa 5MB.");

                await using var stream = request.AvatarFile.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(request.AvatarFile.FileName, stream),
                    Folder = "ProjectSEP490/Profile/user_avatars"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                user.Avatar = uploadResult.SecureUrl.ToString();
            }

            await context.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully." });
        }

    }
}
