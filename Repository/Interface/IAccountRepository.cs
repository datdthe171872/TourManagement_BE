using TourManagement_BE.Data.DTO.Response.AccountResponse;
using TourManagement_BE.Data;

namespace TourManagement_BE.Repository.Interface
{
    public interface IAccountRepository
    {
        Task<List<ListAccountResponse>> ListAllAccount();
    }
}
