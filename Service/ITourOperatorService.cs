using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service;

public interface ITourOperatorService
{
    Task<TourOperatorListResponse> GetTourOperatorsAsync(TourOperatorSearchRequest request);
    Task<TourOperatorDetailResponse?> GetTourOperatorDetailAsync(int id);
}