using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Helper.Common;
using TourManagement_BE.Service;

namespace TourManagement_BE.Controllers;

[ApiController]
[Route("api/[controller]")]

public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly JwtHelper _jwtHelper;

    public NotificationController(INotificationService notificationService, JwtHelper jwtHelper)
    {
        _notificationService = notificationService;
        _jwtHelper = jwtHelper;
    }

    /// <summary>
    /// Lấy danh sách notifications của user đã đăng nhập
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<NotificationListResponse>> GetUserNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            if (userId == null)
            {
                return Unauthorized("Token không hợp lệ");
            }

            var result = await _notificationService.GetUserNotificationsAsync(userId.Value, page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
        }
    }

    /// <summary>
    /// Lấy chi tiết một notification
    /// </summary>
    [HttpGet("{notificationId}")]
    public async Task<ActionResult<NotificationResponse>> GetNotificationById(int notificationId)
    {
        try
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            if (userId == null)
            {
                return Unauthorized("Token không hợp lệ");
            }

            var notification = await _notificationService.GetNotificationByIdAsync(notificationId, userId.Value);
            if (notification == null)
            {
                return NotFound("Không tìm thấy notification");
            }

            return Ok(notification);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
        }
    }

    /// <summary>
    /// Đánh dấu một notification đã đọc
    /// </summary>
    [HttpPut("{notificationId}/mark-read")]
    public async Task<ActionResult> MarkNotificationAsRead(int notificationId)
    {
        try
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            if (userId == null)
            {
                return Unauthorized("Token không hợp lệ");
            }

            var result = await _notificationService.MarkNotificationAsReadAsync(notificationId, userId.Value);
            if (!result)
            {
                return NotFound("Không tìm thấy notification");
            }

            return Ok(new { message = "Đã đánh dấu notification đã đọc" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
        }
    }

    /// <summary>
    /// Đánh dấu tất cả notifications đã đọc
    /// </summary>
    [HttpPut("mark-all-read")]
    public async Task<ActionResult> MarkAllNotificationsAsRead()
    {
        try
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            if (userId == null)
            {
                return Unauthorized("Token không hợp lệ");
            }

            await _notificationService.MarkAllNotificationsAsReadAsync(userId.Value);
            return Ok(new { message = "Đã đánh dấu tất cả notifications đã đọc" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
        }
    }

    /// <summary>
    /// Lấy số lượng notifications chưa đọc
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadNotificationCount()
    {
        try
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            if (userId == null)
            {
                return Unauthorized("Token không hợp lệ");
            }

            var count = await _notificationService.GetUnreadNotificationCountAsync(userId.Value);
            return Ok(count);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
        }
    }
} 