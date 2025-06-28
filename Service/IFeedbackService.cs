using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service;

public interface IFeedbackService
{
    Task<FeedbackListResponse> GetFeedbacksAsync(FeedbackSearchRequest request);
    Task<FeedbackResponse?> GetFeedbackDetailAsync(int id);
    Task<FeedbackResponse> CreateFeedbackAsync(CreateFeedbackRequest request);
    Task<FeedbackResponse?> UpdateFeedbackAsync(int id, UpdateFeedbackRequest request);
    Task<bool> SoftDeleteFeedbackAsync(int id);
} 