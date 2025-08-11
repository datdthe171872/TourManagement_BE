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

    public async Task CreateTourAcceptanceReportNotificationAsync(int userId, int reportId)
    {
        await CreateNotificationAsync(
            userId,
            "Báo cáo hoàn thành tour mới",
            "Bạn có một báo cáo hoàn thành tour mới. Hãy kiểm tra để biết thêm chi tiết.",
            "TourAcceptanceReport",
            reportId.ToString()
        );
    }

    public async Task CreateFeedbackViolationNotificationAsync(int userId, int feedbackId)
    {
        await CreateNotificationAsync(
            userId,
            "Vi phạm tiêu chuẩn cộng đồng",
            "Feedback của bạn đã vi phạm tiêu chuẩn cộng đồng và đã bị ẩn.",
            "FeedbackViolation",
            feedbackId.ToString()
        );
    }

    public async Task CreateFeedbackReportNotificationAsync(int adminUserId, int ratingId, string reason, int tourOperatorId)
    {
        // Lấy thông tin chi tiết của feedback
        var feedback = await _context.TourRatings
            .Include(tr => tr.User)
            .Include(tr => tr.Tour)
            .FirstOrDefaultAsync(tr => tr.RatingId == ratingId);

        // Lấy thông tin chi tiết của tour operator
        var tourOperator = await _context.TourOperators
            .Include(to => to.User)
            .FirstOrDefaultAsync(to => to.TourOperatorId == tourOperatorId);

        string feedbackContent = feedback?.Comment ?? "Nội dung không khả dụng";
        string tourOperatorName = tourOperator?.User?.UserName ?? "Tour Operator không xác định";
        string tourName = feedback?.Tour?.Title ?? "Tour không xác định";

        await CreateNotificationAsync(
            adminUserId,
            "Báo cáo Feedback mới",
            $"Có một feedback về tour '{tourName}' bị báo cáo bởi {tourOperatorName}. Nội dung feedback: '{feedbackContent}'. Lý do báo cáo: {reason}",
            "FeedbackReport",
            ratingId.ToString()
        );
    }

    public async Task CreatePaymentOverdueNotificationAsync(int userId, int bookingId, DateTime paymentDueDate)
    {
        await CreateNotificationAsync(
            userId,
            "Quá hạn thanh toán",
            $"Booking #{bookingId} đã quá hạn thanh toán. Hạn cuối thanh toán: {paymentDueDate:dd/MM/yyyy}. Booking đã bị hủy tự động.",
            "PaymentOverdue",
            bookingId.ToString()
        );
    }

    public async Task CreateTourOperatorNotificationAsync(int userId, int bookingId, string title, string message)
    {
        await CreateNotificationAsync(
            userId,
            title,
            message,
            "TourOperatorNotification",
            bookingId.ToString()
        );
    }
} 
//abc