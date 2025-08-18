using System.Threading.Tasks;
using TourManagement_BE.Data.DTO.Request.Chat;
using TourManagement_BE.Data.DTO.Response.Chat;

namespace TourManagement_BE.Service
{
	public interface IChatbotService
	{
		Task<ChatResponse> ProcessMessageAsync(ChatRequest request);
		Task<TourDetail?> GetTourDetailAsync(int tourId);
		Task<object> CompareTwoToursAsync(int tour1Id, int tour2Id);
		Task<object> PlanOrCreateBookingAsync(BookingPlan plan, int userId);
		Task<object?> GetBookingStatusAsync(int bookingId, int userId);
	}
}


