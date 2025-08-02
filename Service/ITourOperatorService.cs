using Data.DTO.Response;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service;

public interface ITourOperatorService
{
    Task<TourOperatorListResponse> GetTourOperatorsAsync(TourOperatorSearchRequest request);
    Task<TourOperatorDetailResponse?> GetTourOperatorDetailAsync(int id);
    Task<TourOperatorDetailResponse> CreateTourOperatorAsync(CreateTourOperatorRequest request);
    Task<TourOperatorDetailResponse?> UpdateTourOperatorAsync(int id, UpdateTourOperatorRequest request);
    Task<bool> SoftDeleteTourOperatorAsync(int id);
    Task<TourOperatorDashboardResponse> GetDashboardStats(int operatorId);
    Task<TourGuideListResponse> GetTourGuidesAsync(int tourOperatorId, TourGuideSearchRequest request);
}