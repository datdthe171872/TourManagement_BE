using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Service;

namespace TourManagement_BE.Service;

public class NotificationService : INotificationService
{
    private readonly MyDBContext _context;

    public NotificationService(MyDBContext context)
    {
        _context = context;
    }

    public async Task<NotificationListResponse> GetUserNotificationsAsync(int userId, int page = 1, int pageSize = 10)
    {
        var query = _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt);

        var totalCount = await query.CountAsync();
        var unreadCount = await query.CountAsync(n => !n.IsRead);

        var notifications = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificationResponse
            {
                NotificationId = n.NotificationId,
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                RelatedEntityId = n.RelatedEntityId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync();

        return new NotificationListResponse
        {
            Notifications = notifications,
            TotalCount = totalCount,
            UnreadCount = unreadCount
        };
    }

    public async Task<NotificationResponse?> GetNotificationByIdAsync(int notificationId, int userId)
    {
        var notification = await _context.Notifications
            .Where(n => n.NotificationId == notificationId && n.UserId == userId)
            .Select(n => new NotificationResponse
            {
                NotificationId = n.NotificationId,
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                RelatedEntityId = n.RelatedEntityId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            })
            .FirstOrDefaultAsync();

        return notification;
    }

    public async Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

        if (notification == null)
            return false;

        notification.IsRead = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAllNotificationsAsReadAsync(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetUnreadNotificationCountAsync(int userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task CreateNotificationAsync(int userId, string title, string message, string type, string? relatedEntityId = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            RelatedEntityId = relatedEntityId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    public async Task CreateBookingSuccessNotificationAsync(int userId, int bookingId)
    {
        await CreateNotificationAsync(
            userId,
            "Đặt tour thành công!",
            "Bạn đã đặt tour thành công. Chúng tôi sẽ liên hệ với bạn sớm nhất để xác nhận chi tiết.",
            "Booking",
            bookingId.ToString()
        );
    }

    public async Task CreateGuideNoteNotificationAsync(int userId, int guideNoteId)
    {
        await CreateNotificationAsync(
            userId,
            "Ghi chú hướng dẫn mới",
            "Bạn có một ghi chú hướng dẫn mới. Hãy kiểm tra để biết thêm chi tiết.",
            "GuideNote",
            guideNoteId.ToString()
        );
    }

    public async Task CreateFeedbackNotificationAsync(int userId, int feedbackId)
    {
        await CreateNotificationAsync(
            userId,
            "Phản hồi mới",
            "Bạn có một phản hồi mới. Hãy kiểm tra để biết thêm chi tiết.",
            "Feedback",
            feedbackId.ToString()
        );
    }

    public async Task CreateRegistrationSuccessNotificationAsync(int userId)
    {
        await CreateNotificationAsync(
            userId,
            "Đăng ký thành công!",
            "Chào mừng bạn đến với hệ thống quản lý tour. Tài khoản của bạn đã được tạo thành công.",
            "Registration"
        );
    }
} 