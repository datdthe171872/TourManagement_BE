using System.Threading.Tasks;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service
{
    public interface IExtraChargeService
    {
        Task<ExtraChargeListResponse> GetAllAsync(bool? isActive = null);
        Task<ExtraChargeResponse> CreateAsync(CreateExtraChargeRequest request);
        Task<ExtraChargeResponse> UpdateAsync(UpdateExtraChargeRequest request);
        Task<bool> DeleteAsync(int extraChargeId);
    }
} 