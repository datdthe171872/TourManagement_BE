using TourManagement_BE.Data.DTO.Response.PaymentResponse;

namespace TourManagement_BE.Service.PaymentHistory
{
    public interface IPaymentService
    {
        Task<PagedResult<ViewAllPaymentHistory>> GetAllUserPaymentHistoryAsync(int pageNumber, int pageSize);
        Task<PagedResult<ViewAllPaymentHistory>> SearchAllUserPaymentHistoryAsync(string keyword, int pageNumber, int pageSize);
        Task<PagedResult<ViewHistoryPaymentPackageResponse>> GetAllTourOperatorPaymentHistoryAsync(int pageNumber, int pageSize);
        Task<PagedResult<ViewHistoryPaymentPackageResponse>> SearchAllTourOperatorPaymentHistoryAsync(string keyword, int pageNumber, int pageSize);
        Task<PagedResult<ViewHistoryPaymentPackageResponse>> GetPaymentPackageHistoryByTourOperatorAsync(int userId, int pageNumber, int pageSize);
        Task<PagedResult<ViewPaymentResponse>> GetUserPaymentDetailHistoryAsync(int userId, int pageNumber, int pageSize);
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
