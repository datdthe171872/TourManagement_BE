using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Service;
using TourManagement_BE.Helper.Common;
using TourManagement_BE.Data.Context;
using System.Security.Claims;

namespace TourManagement_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;
    private readonly JwtHelper _jwtHelper;
    private readonly MyDBContext _context;

    public FeedbackController(IFeedbackService feedbackService, JwtHelper jwtHelper, MyDBContext context)
    {
        _feedbackService = feedbackService;
        _jwtHelper = jwtHelper;
        _context = context;
    }

    /// <summary>
    /// Admin: Lấy tất cả feedback với tìm kiếm theo Username, RatingId và sắp xếp từ mới nhất
    /// </summary>
    /// <param name="request">Thông tin tìm kiếm và phân trang</param>
    /// <returns>Danh sách feedback</returns>
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdminFeedbacks([FromQuery] AdminFeedbackSearchRequest request)
    {
        try
        {
            var result = await _feedbackService.GetAdminFeedbacksAsync(request);
            if (result.Feedbacks == null || !result.Feedbacks.Any())
            {
                return Ok(new {
                    message = "Không tìm thấy feedback nào phù hợp với điều kiện tìm kiếm.",
                    data = result
                });
            }
            return Ok(new {
                message = "Lấy danh sách feedback thành công",
                data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy danh sách feedback", error = ex.Message });
        }
    }

    /// <summary>
    /// Tour Operator: Lấy feedback về các tour của Tour Operator
    /// </summary>
    /// <param name="request">Thông tin tìm kiếm và phân trang</param>
    /// <returns>Danh sách feedback</returns>
    [HttpGet("tour-operator")]
    [Authorize(Roles = "Tour Operator")]
    public async Task<IActionResult> GetTourOperatorFeedbacks([FromQuery] TourOperatorFeedbackSearchRequest request)
    {
        try
        {
            var operatorUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tourOperator = await _context.TourOperators
                .FirstOrDefaultAsync(o => o.UserId == operatorUserId && o.IsActive);
            
            if (tourOperator == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin TourOperator cho user này." });
            }

            var result = await _feedbackService.GetTourOperatorFeedbacksAsync(tourOperator.TourOperatorId, request);
            if (result.Feedbacks == null || !result.Feedbacks.Any())
            {
                return Ok(new {
                    message = "Không tìm thấy feedback nào cho các tour của bạn.",
                    data = result
                });
            }
            return Ok(new {
                message = "Lấy danh sách feedback thành công",
                data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy danh sách feedback", error = ex.Message });
        }
    }

    /// <summary>
    /// Tạo mới feedback với upload ảnh
    /// </summary>
    /// <param name="request">Thông tin feedback cần tạo</param>
    /// <returns>Thông tin feedback đã tạo</returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateFeedback([FromForm] CreateFeedbackRequest request)
    {
        try
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            if (userId == null || userId == 0)
            {
                return Unauthorized(new { message = "Token không hợp lệ hoặc đã hết hạn" });
            }

            var result = await _feedbackService.CreateFeedbackAsync(request, userId.Value);
            return CreatedAtAction(nameof(GetMyFeedbacks), new { }, new { 
                message = "Tạo feedback thành công", 
                data = result 
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi tạo feedback", error = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật thông tin feedback
    /// </summary>
    /// <param name="id">ID của feedback cần cập nhật</param>
    /// <param name="request">Thông tin cập nhật</param>
    /// <returns>Thông tin feedback đã cập nhật</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFeedback(int id, [FromBody] UpdateFeedbackRequest request)
    {
        try
        {
            var result = await _feedbackService.UpdateFeedbackAsync(id, request);
            if (result == null)
            {
                return NotFound(new { message = "Không tìm thấy feedback với id này." });
            }

            return Ok(new { message = "Cập nhật feedback thành công", data = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi cập nhật feedback", error = ex.Message });
        }
    }

    /// <summary>
    /// Xóa mềm feedback
    /// </summary>
    /// <param name="id">ID của feedback cần xóa</param>
    /// <returns>Kết quả xóa</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeleteFeedback(int id)
    {
        try
        {
            var result = await _feedbackService.SoftDeleteFeedbackAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy feedback với id này." });
            }

            return Ok(new { message = "Xóa feedback thành công" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi xóa feedback", error = ex.Message });
        }
    }

    /// <summary>
    /// Lấy tất cả feedback của user đã đăng nhập
    /// </summary>
    /// <returns>Danh sách feedback của user</returns>
    [HttpGet("my-feedbacks")]
    [Authorize]
    public async Task<IActionResult> GetMyFeedbacks()
    {
        try
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            if (userId == null || userId == 0)
            {
                return Unauthorized(new { message = "Token không hợp lệ hoặc đã hết hạn" });
            }

            var result = await _feedbackService.GetUserFeedbacksAsync(userId.Value);
            if (result.Feedbacks == null || !result.Feedbacks.Any())
            {
                return Ok(new {
                    message = "Bạn chưa có feedback nào.",
                    data = result
                });
            }
            return Ok(new {
                message = "Lấy danh sách feedback thành công",
                data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy danh sách feedback", error = ex.Message });
        }
    }

    /// <summary>
    /// Tour Operator: Báo cáo feedback về tour của họ
    /// </summary>
    /// <param name="request">Thông tin báo cáo</param>
    /// <returns>Kết quả báo cáo</returns>
    [HttpPost("report")]
    [Authorize(Roles = "Tour Operator")]
    public async Task<IActionResult> ReportFeedback([FromBody] ReportFeedbackRequest request)
    {
        try
        {
            var operatorUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tourOperator = await _context.TourOperators
                .FirstOrDefaultAsync(o => o.UserId == operatorUserId && o.IsActive);
            
            if (tourOperator == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin TourOperator cho user này." });
            }

            var result = await _feedbackService.ReportFeedbackAsync(tourOperator.TourOperatorId, request);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy feedback với id này hoặc feedback không thuộc về tour của bạn." });
            }

            return Ok(new { 
                message = "Báo cáo feedback thành công. Admin sẽ được thông báo về vấn đề này.",
                data = new {
                    RatingId = request.RatingId,
                    TourOperatorId = tourOperator.TourOperatorId
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi báo cáo feedback", error = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật trạng thái feedback (chỉ Admin)
    /// </summary>
    /// <param name="request">Thông tin cập nhật trạng thái</param>
    /// <returns>Kết quả cập nhật</returns>
    [HttpPut("update-status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFeedbackStatus([FromBody] UpdateFeedbackStatusRequest request)
    {
        try
        {
            var result = await _feedbackService.UpdateFeedbackStatusAsync(request.RatingId, request.IsActive);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy feedback với id này." });
            }

            var statusMessage = request.IsActive ? "kích hoạt" : "ẩn";
            return Ok(new { message = $"Cập nhật trạng thái feedback thành công - đã {statusMessage}" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi cập nhật trạng thái feedback", error = ex.Message });
        }
    }
} 