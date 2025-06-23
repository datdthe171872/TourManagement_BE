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
} 