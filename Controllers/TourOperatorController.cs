using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Service;

namespace TourManagement_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TourOperatorController : ControllerBase
{
    private readonly ITourOperatorService _tourOperatorService;

    public TourOperatorController(ITourOperatorService tourOperatorService)
    {
        _tourOperatorService = tourOperatorService;
    }

    /// <summary>
    /// Lấy danh sách tour operator với tìm kiếm theo tên công ty
    /// </summary>
    /// <param name="request">Thông tin tìm kiếm và phân trang</param>
    /// <returns>Danh sách tour operator</returns>
    [HttpGet]
    public async Task<IActionResult> GetTourOperators([FromQuery] TourOperatorSearchRequest request)
    {
        try
        {
            var result = await _tourOperatorService.GetTourOperatorsAsync(request);
            if (result.TourOperators == null || !result.TourOperators.Any())
            {
                return Ok(new {
                    message = "Không tìm thấy tour operator nào phù hợp với từ khóa tìm kiếm.",
                    data = result
                });
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy danh sách tour operator", error = ex.Message });
        }
    }

    /// <summary>
    /// Lấy chi tiết tour operator theo ID
    /// </summary>
    /// <param name="id">ID của tour operator</param>
    /// <returns>Chi tiết tour operator</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTourOperatorDetail(int id)
    {
        try
        {
            var result = await _tourOperatorService.GetTourOperatorDetailAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin tour operator với id này." });
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy chi tiết tour operator", error = ex.Message });
        }
    }

    /// <summary>
    /// Tạo mới tour operator
    /// </summary>
    /// <param name="request">Thông tin tour operator cần tạo</param>
    /// <returns>Thông tin tour operator đã tạo</returns>
    [HttpPost]
    public async Task<IActionResult> CreateTourOperator([FromBody] CreateTourOperatorRequest request)
    {
        try
        {
            var result = await _tourOperatorService.CreateTourOperatorAsync(request);
            return CreatedAtAction(nameof(GetTourOperatorDetail), new { id = result.TourOperatorId }, new { 
                message = "Tạo tour operator thành công", 
                data = result 
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi tạo tour operator", error = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật thông tin tour operator
    /// </summary>
    /// <param name="id">ID của tour operator cần cập nhật</param>
    /// <param name="request">Thông tin cập nhật</param>
    /// <returns>Thông tin tour operator đã cập nhật</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTourOperator(int id, [FromBody] UpdateTourOperatorRequest request)
    {
        try
        {
            var result = await _tourOperatorService.UpdateTourOperatorAsync(id, request);
            if (result == null)
            {
                return NotFound(new { message = "Không tìm thấy tour operator với id này." });
            }

            return Ok(new { message = "Cập nhật tour operator thành công", data = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi cập nhật tour operator", error = ex.Message });
        }
    }

    /// <summary>
    /// Xóa mềm tour operator
    /// </summary>
    /// <param name="id">ID của tour operator cần xóa</param>
    /// <returns>Kết quả xóa</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeleteTourOperator(int id)
    {
        try
        {
            var result = await _tourOperatorService.SoftDeleteTourOperatorAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy tour operator với id này." });
            }

            return Ok(new { message = "Xóa tour operator thành công" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi xóa tour operator", error = ex.Message });
        }
    }

    /// <summary>
    /// Lấy thống kê dashboard cho tour operator
    /// </summary>
    /// <param name="id">ID của tour operator</param>
    /// <returns>Thống kê dashboard</returns>
    [HttpGet("{id}/dashboard")]
    public async Task<IActionResult> GetDashboard(int id)
    {
        try
        {
            var result = await _tourOperatorService.GetDashboardStats(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy thống kê dashboard", error = ex.Message });
        }
    }
} 