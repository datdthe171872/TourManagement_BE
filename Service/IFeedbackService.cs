using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service;

public interface IFeedbackService
{
    Task<FeedbackListResponse> GetFeedbacksAsync(FeedbackSearchRequest request);
    Task<FeedbackResponse?> GetFeedbackDetailAsync(int id);
    Task<FeedbackResponse> CreateFeedbackAsync(CreateFeedbackRequest request, int userId);
    Task<FeedbackResponse?> UpdateFeedbackAsync(int id, UpdateFeedbackRequest request);
    Task<bool> SoftDeleteFeedbackAsync(int id);
    Task<bool> UpdateFeedbackStatusAsync(int ratingId, bool isActive);
    
    // New method for user feedback
    Task<FeedbackListResponse> GetUserFeedbacksAsync(int userId);
    
    // New methods for Admin and Tour Operator
    Task<FeedbackListResponse> GetAdminFeedbacksAsync(AdminFeedbackSearchRequest request);
    Task<FeedbackListResponse> GetTourOperatorFeedbacksAsync(int tourOperatorId, TourOperatorFeedbackSearchRequest request);
    Task<bool> ReportFeedbackAsync(int? tourOperatorId, ReportFeedbackRequest request);
    
    // New method for getting all feedbacks with TourId search
    Task<FeedbackListResponse> GetAllFeedbacksAsync(AllFeedbackSearchRequest request);
} 