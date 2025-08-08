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
        Task CreateAsync(CreateServicePackageRequest request);
        Task AddFeatureAsync(AddServicePackageFeatureRequest request);
        Task UpdateAsync(UpdateServicePackageRequest request);
        Task UpdateFeatureAsync(UpdateServicePackageFeatureRequest request);
        Task ToggleStatusAsync(int packageId);
        Task ToggleFeatureStatusAsync(int featureId);
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
}