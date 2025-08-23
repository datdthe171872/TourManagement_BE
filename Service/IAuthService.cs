using TourManagement_BE.Data.DTO.Request;
using System.Threading.Tasks;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task RegisterAsync(RegisterRequest request, int? tourOperatorId = null);
        Task ForgotPasswordAsync(ForgotPasswordRequest request);
        Task ResetPasswordAsync(ResetPasswordRequest request);

        Task<LoginResponse> AuthWithGoogleAsync(string token);
    }
}
