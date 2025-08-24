using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.DepartureDatesRequest;
using TourManagement_BE.Data.DTO.Response.DepartureDateResponse;
using TourManagement_BE.Helper.Constant;
using TourManagement_BE.Helper.Validator;
using TourManagement_BE.Service;

namespace TourManagement_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartureDatesController : ControllerBase
{
    private readonly IDepartureDateService _departureDateService;
    private readonly CreateDepartureDateRequestValidator _validator;
    private readonly MyDBContext _context;

    public DepartureDatesController(
        IDepartureDateService departureDateService,
        CreateDepartureDateRequestValidator validator,
        MyDBContext context)
    {
        _departureDateService = departureDateService;
        _validator = validator;
        _context = context;
    }

    /// <summary>
    /// Tạo các ngày khởi hành cho tour
    /// </summary>
    /// <param name="request">Thông tin tạo ngày khởi hành</param>
    /// <returns>Kết quả tạo ngày khởi hành</returns>
    [HttpPost]
    [Authorize(Roles = Roles.TourOperator)]
    public async Task<IActionResult> CreateDepartureDates([FromBody] CreateDepartureDateRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Message = "Dữ liệu không hợp lệ",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        var result = await _departureDateService.CreateDepartureDatesAsync(request);
        
        if (!result)
        {
            return BadRequest(new
            {
                Message = "Không thể tạo ngày khởi hành. Vui lòng kiểm tra lại thông tin tour và đảm bảo ngày khởi hành cách hôm nay ít nhất 1 tháng."
            });
        }

        return Ok(new
        {
            Message = "Tạo ngày khởi hành thành công"
        });
    }

    /// <summary>
    /// Lấy tất cả các ngày khởi hành
    /// </summary>
    /// <returns>Danh sách tất cả ngày khởi hành</returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllDepartureDates()
    {
        var departureDates = await _departureDateService.GetAllDepartureDatesAsync();
        
        return Ok(new
        {
            Message = "Lấy danh sách ngày khởi hành thành công",
            Data = departureDates
        });
    }

    /// <summary>
    /// Lấy ngày khởi hành theo TourId
    /// </summary>
    /// <param name="tourId">ID của tour</param>
    /// <returns>Danh sách ngày khởi hành của tour</returns>
    [HttpGet("tour/{tourId}")]
    [Authorize]
    public async Task<IActionResult> GetDepartureDatesByTourId(int tourId)
    {
        if (tourId <= 0)
        {
            return BadRequest(new
            {
                Message = "TourId không hợp lệ"
            });
        }

        var departureDates = await _departureDateService.GetDepartureDatesByTourIdAsync(tourId);
        
        return Ok(new
        {
            Message = "Lấy danh sách ngày khởi hành theo tour thành công",
            Data = departureDates
        });
    }

    /// <summary>
    /// Lấy tất cả ngày khởi hành của TourOperator hiện tại
    /// </summary>
    /// <returns>Danh sách tất cả ngày khởi hành của TourOperator</returns>
    [HttpGet("operator")]
    [Authorize(Roles = Roles.TourOperator)]
    public async Task<IActionResult> GetDepartureDatesByTourOperator()
    {
        // Lấy UserId từ JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return BadRequest(new
            {
                Message = "Không thể xác định thông tin user"
            });
        }

        var departureDates = await _departureDateService.GetDepartureDatesByTourOperatorAsync(userId);
        
        return Ok(new
        {
            Message = "Lấy danh sách ngày khởi hành của TourOperator thành công",
            Data = departureDates
        });
    }

    /// <summary>
    /// Lấy ngày khởi hành và thông tin booking cho TourOperator hiện tại
    /// </summary>
    /// <returns>Danh sách ngày khởi hành với thông tin booking</returns>
    [HttpGet("operator/with-bookings")]
    [Authorize(Roles = Roles.TourOperator)]
    public async Task<IActionResult> GetDepartureDatesWithBookingsByTourOperator()
    {
        // Lấy UserId từ JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return BadRequest(new
            {
                Message = "Không thể xác định thông tin user"
            });
        }

        var departureDatesWithBookings = await _departureDateService.GetDepartureDatesWithBookingsByTourOperatorAsync(userId);
        
        return Ok(new
        {
            Message = "Lấy danh sách ngày khởi hành với booking thành công",
            Data = departureDatesWithBookings
        });
    }

    /// <summary>
    /// Lấy tất cả booking trong một DepartureDateId cụ thể của TourOperator hiện tại
    /// </summary>
    /// <param name="departureDateId">ID của ngày khởi hành</param>
    /// <returns>Thông tin ngày khởi hành và danh sách booking</returns>
    [HttpGet("operator/departure-date/{departureDateId}/bookings")]
    [Authorize(Roles = Roles.TourOperator)]
    public async Task<IActionResult> GetBookingsByDepartureDateId(int departureDateId)
    {
        if (departureDateId <= 0)
        {
            return BadRequest(new
            {
                Message = "DepartureDateId không hợp lệ"
            });
        }

        // Lấy UserId từ JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return BadRequest(new
            {
                Message = "Không thể xác định thông tin user"
            });
        }

        var departureDateWithBookings = await _departureDateService.GetBookingsByDepartureDateIdAsync(departureDateId, userId);
        
        if (departureDateWithBookings == null)
        {
            return NotFound(new
            {
                Message = "Không tìm thấy ngày khởi hành hoặc bạn không có quyền truy cập"
            });
        }

        return Ok(new
        {
            Message = "Lấy danh sách booking theo ngày khởi hành thành công",
            Data = departureDateWithBookings
        });
    }

   
} 