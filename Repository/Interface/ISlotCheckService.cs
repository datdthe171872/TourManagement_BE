using TourManagement_BE.Data.DTO.Response.PaymentResponse;

namespace TourManagement_BE.Repository.Interface
{
    public interface ISlotCheckService
    {
        Task<CheckSlotTourOperatorResponse?> CheckRemainingSlotsAsync(int tourOperatorId);
    }
}
