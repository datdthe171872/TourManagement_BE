using TourManagement_BE.Data.DTO.Request.ProfileRequest;
using TourManagement_BE.Data.DTO.Response.ProfileResponse;

namespace TourManagement_BE.Service.Profile
{
    public interface IProfileService
    {
        Task<UserProfileResponse> GetUserProfileAsync(int userId);
        Task<UpdateResult> UpdateUserProfileAsync(UpdateProfileRequest request);
    }
    public class UpdateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserProfileResponse UserProfile { get; set; }
    }
}
