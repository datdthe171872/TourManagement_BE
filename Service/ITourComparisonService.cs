using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service
{
    public interface ITourComparisonService
    {
        Task<TourComparisonResponse> CompareToursAsync(TourComparisonRequest request);
    }
}
