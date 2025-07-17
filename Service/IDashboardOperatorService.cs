using System.Collections.Generic;
using System.Threading.Tasks;

namespace TourManagement_BE.Service
{
    public interface IDashboardOperatorService
    {
        Task<int> GetTotalToursAsync(int userId);
        Task<int> GetTotalBookingsAsync(int userId);
        Task<decimal> GetTotalEarningsAsync(int userId);
        Task<int> GetTotalFeedbacksAsync(int userId);
        Task<List<object>> GetLatestInvoicesAsync(int userId, int count = 5);
    }
} 