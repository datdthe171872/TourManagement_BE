using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service;

public interface INotificationService
{
    Task<NotificationListResponse> GetUserNotificationsAsync(int userId, int page = 1, int pageSize = 10);
    Task<NotificationResponse?> GetNotificationByIdAsync(int notificationId, int userId);
    Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId);
    Task<bool> MarkAllNotificationsAsReadAsync(int userId);
    Task<int> GetUnreadNotificationCountAsync(int userId);
    Task CreateNotificationAsync(int userId, string title, string message, string type, string? relatedEntityId = null);
    Task CreateBookingSuccessNotificationAsync(int userId, int bookingId);
    Task CreateGuideNoteNotificationAsync(int userId, int guideNoteId);
    Task CreateFeedbackNotificationAsync(int userId, int feedbackId);
    Task CreateRegistrationSuccessNotificationAsync(int userId);
} 