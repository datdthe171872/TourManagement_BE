using System;

namespace TourManagement_BE.Data.DTO.Response;

public class NotificationResponse
{
    public int NotificationId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string? RelatedEntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotificationListResponse
{
    public List<NotificationResponse> Notifications { get; set; } = new List<NotificationResponse>();
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
} 