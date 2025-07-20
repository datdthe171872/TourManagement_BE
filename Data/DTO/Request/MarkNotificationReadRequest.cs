using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.DTO.Request;

public class MarkNotificationReadRequest
{
    [Required]
    public int NotificationId { get; set; }
}

public class MarkAllNotificationsReadRequest
{
    public int UserId { get; set; }
} 