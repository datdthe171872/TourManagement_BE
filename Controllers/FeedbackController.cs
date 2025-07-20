using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Service;
using TourManagement_BE.Helper.Common;

namespace TourManagement_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;
    private readonly JwtHelper _jwtHelper;

    public FeedbackController(IFeedbackService feedbackService, JwtHelper jwtHelper)
    {
        _feedbackService = feedbackService;
        _jwtHelper = jwtHelper;
    }

    /// <summary>
    /// Lấy danh sách feedback với tìm kiếm và phân trang
    /// </summary>
    /// <param name="request">Thông tin tìm kiếm và phân trang</param>
    /// <returns>Danh sách feedback</returns>
    [HttpGet]
    public async Task<IActionResult> GetFeedbacks([FromQuery] FeedbackSearchRequest request)
    {
        try
        {
            var result = await _feedbackService.GetFeedbacksAsync(request);
            if (result.Feedbacks == null || !result.Feedbacks.Any())
            {
                return Ok(new {
                    message = "Không tìm thấy feedback nào phù hợp với điều kiện tìm kiếm.",
                    data = result
                });
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy danh sách feedback", error = ex.Message });
        }
    }

    /// <summary>
    /// Lấy chi tiết feedback theo ID
    /// </summary>
    /// <param name="id">ID của feedback</param>
    /// <returns>Chi tiết feedback</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFeedbackDetail(int id)
    {
        try
        {
            var result = await _feedbackService.GetFeedbackDetailAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin feedback với id này." });
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy chi tiết feedback", error = ex.Message });
        }
    }

    /// <summary>
    /// Tạo mới feedback
    /// </summary>
    /// <param name="request">Thông tin feedback cần tạo</param>
    /// <returns>Thông tin feedback đã tạo</returns>
    [HttpPost]
    public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackRequest request)
    {
        try
        {
            var result = await _feedbackService.CreateFeedbackAsync(request);
            return CreatedAtAction(nameof(GetFeedbackDetail), new { id = result.RatingId }, new { 
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


} 