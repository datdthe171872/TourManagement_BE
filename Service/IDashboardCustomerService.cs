using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service
{
    public interface IDashboardCustomerService
    {
        Task<int> GetTotalBookingsAsync(int userId);
        Task<int> GetTotalTransactionsAsync(int userId);
        Task<decimal> GetAverageValueAsync(int userId);
        Task<List<RecentBookingResponse>> GetRecentBookingsAsync(int userId, int count = 5);
        Task<List<RecentInvoiceResponse>> GetRecentInvoicesAsync(int userId, int count = 5);
    }
} 