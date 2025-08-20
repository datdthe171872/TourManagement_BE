using TourManagement_BE.Data.DTO.Request.TourRequest;
using TourManagement_BE.Data.DTO.Response.TourResponse;

namespace TourManagement_BE.Service.TourManagement
{
    public interface ITourService
    {
        //TourOperator
        Task<List<ListTourResponse>> ListAllForTourOperatorAsync(int userId);
        Task<PagedResult<ListTourResponse>> ListAllForTourOperatorPagingAsync(int userId, int pageNumber, int pageSize);
        Task<List<ListTourResponse>> SearchForOperatorAsync(int userId, string keyword);
        Task<PagedResult<ListTourResponse>> SearchPagingForOperatorAsync(int userId, string keyword, int pageNumber, int pageSize);
        Task<PagedResult<ListTourResponse>> FilterForOperatorPagingAsync(int userId, TourFilterRequest filter, int pageNumber, int pageSize);
        Task<List<ListTourResponse>> FilterForOperatorAsync(int userId, TourFilterRequest filter);
        Task<TourDetailResponse> GetDetailForOperatorAsync(int tourId, bool forUpdate = false);

        //Customer

        Task<PagedResult<ListTourResponse>> TourOperatorFullTourListPagingAsync(int touroperatorid, int pageNumber, int pageSize);
        Task<PagedResult<ListTourResponse>> SearchPagingTourOperatorFullTourListAsync(int touroperatorid, string keyword, int pageNumber, int pageSize);
        Task<PagedResult<ListTourResponse>> FilterPagingTourOperatorFullTourListAsync(int touroperatorid, TourFilterRequest filter, int pageNumber, int pageSize);
        Task<List<ListTourResponse>> ListAllForCustomerAsync();
        Task<PagedResult<ListTourResponse>> ListAllForCustomerPagingAsync(int pageNumber, int pageSize);
        Task<List<ListTourResponse>> SearchForCustomerAsync(string keyword);
        Task<PagedResult<ListTourResponse>> SearchPagingForCustomerAsync(string keyword, int pageNumber, int pageSize);
        Task<PagedResult<ListTourResponse>> FilterForCustomerPagingAsync(TourFilterRequest filter, int pageNumber, int pageSize);
        Task<List<ListTourResponse>> FilterForCustomerAsync(TourFilterRequest filter);
        Task<TourDetailResponse> GetDetailForCustomerAsync(int tourId);

        /*Task<CreateTourResult> CreateAsync(TourCreateRequest request);
        Task<UpdateTourResult> UpdateAsync(TourUpdateRequest request);*/
        Task<ToggleStatusResult> ToggleStatusAsync(int tourId);
    }

    public class PagedResult<T>
    {
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; }
    }

    public class CreateTourResult
    {
        public string Message { get; set; }
        public int TourId { get; set; }
    }

    public class UpdateTourResult
    {
        public string Message { get; set; }
    }

    public class ToggleStatusResult
    {
        public string Message { get; set; }
        public bool NewStatus { get; set; }
        public string TourStatus { get; set; }
    }

    public class TourFilterRequest
    {
        public string Title { get; set; }
        public string Transportation { get; set; }
        public string StartPoint { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int[] Ratings { get; set; }
    }
}
