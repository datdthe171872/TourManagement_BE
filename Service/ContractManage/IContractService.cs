using TourManagement_BE.Data.DTO.Request.TourContract;
using TourManagement_BE.Data.DTO.Response.ContractTourBooking;

namespace TourManagement_BE.Service.ContractManage
{
    public interface IContractService
    {
        Task<ContractTourBookingResponse> GetContractByBookingIdAsync(int bookingId);
        Task<OperationResult> CreateContractAsync(CreateContractRequest request);
        Task<OperationResult> UpdateContractAsync(UpdateTourContractRequest request);
        Task<OperationResult> DeleteContractAsync(int bookingId);
    }

    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ContractTourBookingResponse Contract { get; set; }
    }
}
