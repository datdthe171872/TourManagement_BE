using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.ProfileRequest;
using TourManagement_BE.Data.DTO.Response.ProfileResponse;

namespace TourManagement_BE.Service.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly MyDBContext _context;
        private readonly Cloudinary _cloudinary;

        public ProfileService(MyDBContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        public async Task<UserProfileResponse> GetUserProfileAsync(int userId)
        {
            var user = await _context.Users
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
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            var tourOperator = await _context.TourOperators.FirstOrDefaultAsync(t => t.UserId == userId);
            if (tourOperator != null)
            {
                user.CompanyName = tourOperator.CompanyName;
                user.Description = tourOperator.Description;
            }

            var tourGuide = await _context.TourGuides.FirstOrDefaultAsync(t => t.UserId == userId);
            if (tourGuide != null)
            {
                user.HomeTourGuideId = tourGuide.TourOperatorId;
            }

            return user;
        }

        public async Task<UpdateResult> UpdateUserProfileAsync(UpdateProfileRequest request)
        {
            var result = new UpdateResult();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
            if (user == null)
            {
                result.Success = false;
                result.Message = "User not found.";
                return result;
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
                    result.Success = false;
                    result.Message = "Invalid email format.";
                    return result;
                }

                var emailExists = await _context.Users.AnyAsync(x => x.Email == request.Email && x.UserId != request.UserId);
                if (emailExists)
                {
                    result.Success = false;
                    result.Message = "Email is already in use by another user.";
                    return result;
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
                // Kiểm tra kích thước file (5MB)
                if (request.AvatarFile.Length > 5 * 1024 * 1024)
                {
                    result.Success = false;
                    result.Message = "Maximum image size is 5MB.";
                    return result;
                }

                // Kiểm tra loại file (chỉ chấp nhận ảnh)
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(request.AvatarFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    result.Success = false;
                    result.Message = "Only image files are allowed (JPG, JPEG, PNG, GIF).";
                    return result;
                }

                // Kiểm tra content type (thêm một lớp bảo vệ)
                var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!allowedContentTypes.Contains(request.AvatarFile.ContentType.ToLowerInvariant()))
                {
                    result.Success = false;
                    result.Message = "Invalid file type. Only image files are allowed.";
                    return result;
                }

                try
                {
                    await using var stream = request.AvatarFile.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(request.AvatarFile.FileName, stream),
                        Folder = "ProjectSEP490/Profile/user_avatars"
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    user.Avatar = uploadResult.SecureUrl.ToString();
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = $"Error uploading image: {ex.Message}";
                    return result;
                }
            }

            await _context.SaveChangesAsync();

            result.Success = true;
            result.Message = "Profile updated successfully.";
            result.UserProfile = await GetUserProfileAsync(request.UserId);
            return result;
        }
    }
}
