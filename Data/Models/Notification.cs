using System;
using System.ComponentModel.DataAnnotations;

namespace TourManagement_BE.Data.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(500)]
    public string Message { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string Type { get; set; } = null!; // Booking, GuideNote, Feedback, Registration, etc.

    public string? RelatedEntityId { get; set; } // ID của entity liên quan (BookingId, GuideNoteId, etc.)

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;
} 