using TourManagement_BE.Data.DTO.Request.ServicePackageRequest;
using TourManagement_BE.Data.DTO.Response.PaymentResponse;
using TourManagement_BE.Data.DTO.Response.ServicePackage;

namespace TourManagement_BE.Service.ServicePackageService
{
    public interface IServicePackageService
    {
        Task<List<ListServicePackageResponse>> ListAllForAdminAsync();
        Task<PagedResult<ListServicePackageResponse>> ListAllPaginatedForAdminAsync(int pageNumber, int pageSize);
        Task<List<ListServicePackageResponse>> ListAllForCustomerAsync();
        Task<PagedResult<ListServicePackageResponse>> ListAllPaginatedForCustomerAsync(int pageNumber, int pageSize);
        Task<ServiceResult> CreateAsync(CreateServicePackageRequest request);
        Task<ServiceResult> AddFeatureAsync(AddServicePackageFeatureRequest request);
        Task<ServiceResult> UpdateAsync(UpdateServicePackageRequest request);
        Task<ServiceResult> UpdateFeatureAsync(UpdateServicePackageFeatureRequest request);
        Task<ServiceResult> ToggleStatusAsync(int packageId);
        Task<ServiceResult> ToggleFeatureStatusAsync(int featureId);
        Task<ListServicePackageResponse> GetDetailForCustomerAsync(int packageId);
        Task<ListServicePackageResponse> GetDetailForAdminAsync(int packageId);
        Task<CheckSlotTourOperatorResponse> CheckSlotForTourOperatorAsync(int userId);
    }

    public class PagedResult<T>
    {
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; }
    }
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}