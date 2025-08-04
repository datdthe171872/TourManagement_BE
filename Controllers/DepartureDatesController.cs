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
                Message = "Không thể tạo ngày khởi hành. Vui lòng kiểm tra lại thông tin tour và ngày bắt đầu."
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
    //[HttpGet("operator/with-bookings")]
    //[Authorize(Roles = Roles.TourOperator)]
    //public async Task<IActionResult> GetDepartureDatesWithBookingsByTourOperator()
    //{
    //    // Lấy UserId từ JWT token
    //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //    if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
    //    {
    //        return BadRequest(new
    //        {
    //            Message = "Không thể xác định thông tin user"
    //        });
    //    }

    //    var departureDatesWithBookings = await _departureDateService.GetDepartureDatesWithBookingsByTourOperatorAsync(userId);
        
    //    return Ok(new
    //    {
    //        Message = "Lấy danh sách ngày khởi hành với booking thành công",
    //        Data = departureDatesWithBookings
    //    });
    //}

    /// <summary>
    /// Lấy tất cả booking trong một DepartureDateId cụ thể của TourOperator hoặc TourGuide hiện tại
    /// </summary>
    /// <param name="departureDateId">ID của ngày khởi hành</param>
    /// <returns>Thông tin ngày khởi hành và danh sách booking</returns>
    [HttpGet("departure-date/{departureDateId}/bookings")]
    [Authorize(Roles = Roles.TourOperator + "," + Roles.TourGuide)]
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

    /// <summary>
    /// Hủy ngày khởi hành và cập nhật trạng thái booking thành Cancelled
    /// </summary>
    /// <param name="departureDateId">ID của ngày khởi hành cần hủy</param>
    /// <returns>Kết quả hủy ngày khởi hành</returns>
    [HttpPut("{departureDateId}/cancel")]
    [Authorize(Roles = Roles.TourOperator)]
    public async Task<IActionResult> CancelDepartureDate(int departureDateId)
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

        var result = await _departureDateService.CancelDepartureDateAsync(departureDateId, userId);
        
        if (!result)
        {
            return BadRequest(new
            {
                Message = "Không thể hủy ngày khởi hành. Vui lòng kiểm tra lại thông tin hoặc ngày khởi hành đã diễn ra."
            });
        }



        return Ok(new
        {
            Message = "Hủy ngày khởi hành thành công. Tất cả booking trong ngày khởi hành này đã được cập nhật thành Cancelled."
        });
    }

    /// <summary>
    /// Lấy danh sách các ngày khởi hành đã bị hủy của TourOperator hiện tại
    /// </summary>
    /// <returns>Danh sách ngày khởi hành đã bị hủy</returns>
    [HttpGet("operator/cancelled")]
    [Authorize(Roles = Roles.TourOperator)]
    public async Task<IActionResult> GetCancelledDepartureDatesByTourOperator()
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

        var cancelledDepartureDates = await _departureDateService.GetCancelledDepartureDatesByTourOperatorAsync(userId);
        
        return Ok(new
        {
            Message = "Lấy danh sách ngày khởi hành đã bị hủy thành công",
            Data = cancelledDepartureDates
        });
    }

    /// <summary>
    /// Bật lại ngày khởi hành đã bị hủy (chỉ áp dụng cho ngày khởi hành cách hiện tại ít nhất 5 ngày)
    /// </summary>
    /// <param name="departureDateId">ID của ngày khởi hành cần bật lại</param>
    /// <returns>Kết quả bật lại ngày khởi hành</returns>
    [HttpPut("{departureDateId}/reactivate")]
    [Authorize(Roles = Roles.TourOperator)]
    public async Task<IActionResult> ReactivateDepartureDate(int departureDateId)
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

        var result = await _departureDateService.ReactivateDepartureDateAsync(departureDateId, userId);
        
        if (!result)
        {
            return BadRequest(new
            {
                Message = "Không thể bật lại ngày khởi hành. Vui lòng kiểm tra lại thông tin hoặc ngày khởi hành phải cách hiện tại ít nhất 5 ngày."
            });
        }



        return Ok(new
        {
            Message = "Bật lại ngày khởi hành thành công. Tất cả booking trong ngày khởi hành này đã được khôi phục thành Pending."
        });
    }

    /// <summary>
    /// Lấy tất cả ngày khởi hành của TourGuide hiện tại
    /// </summary>
    /// <returns>Danh sách tất cả ngày khởi hành của TourGuide</returns>
    [HttpGet("guide")]
    [Authorize(Roles = Roles.TourGuide)]
    public async Task<IActionResult> GetDepartureDatesByTourGuide()
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

        var departureDates = await _departureDateService.GetDepartureDatesByTourGuideAsync(userId);
        
        return Ok(new
        {
            Message = "Lấy danh sách ngày khởi hành của TourGuide thành công",
            Data = departureDates
        });
    }
} 