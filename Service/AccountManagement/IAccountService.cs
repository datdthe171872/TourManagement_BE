using TourManagement_BE.Data.DTO.Request.AccountRequest;
using TourManagement_BE.Data.DTO.Response.AccountResponse;

namespace TourManagement_BE.Service.AccountManagement
{
    public interface IAccountService
    {
        Task<IEnumerable<ListAccountResponse>> GetAllAccountsAsync();
        Task<IEnumerable<ListAccountResponse>> SearchAccountsAsync(string keyword);
        Task<PagedResult<ListAccountResponse>> GetAllAccountsPagedAsync(int pageNumber, int pageSize);
        Task<PagedResult<ListAccountResponse>> SearchAccountsPagedAsync(string keyword, int pageNumber, int pageSize);
        Task<UpdateResult> UpdateAccountStatusAsync(UpdateStatusRequest request);
        Task<UpdateResult> ToggleAccountStatusAsync(int userId);
    }

    public class UpdateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ListAccountResponse Account { get; set; }
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
