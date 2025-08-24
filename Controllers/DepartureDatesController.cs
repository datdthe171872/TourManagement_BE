using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
    private readonly UpdateDepartureDateRequestValidator _updateValidator;
    private readonly MyDBContext _context;

    public DepartureDatesController(
        IDepartureDateService departureDateService,
        CreateDepartureDateRequestValidator validator,
        UpdateDepartureDateRequestValidator updateValidator,
        MyDBContext context)
    {
        _departureDateService = departureDateService;
        _validator = validator;
        _updateValidator = updateValidator;
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
        // Lấy UserId từ JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return BadRequest(new
            {
                Message = "Không thể xác định thông tin user"
            });
        }

        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Message = "Dữ liệu không hợp lệ",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        // Kiểm tra ngày khởi hành phải cách hôm nay ít nhất 1 tháng
        var minimumDate = DateTime.Now.Date.AddMonths(1);
        if (request.StartDate.Date < minimumDate)
        {
            return BadRequest(new
            {
                Message = $"Ngày khởi hành phải cách hôm nay ít nhất 1 tháng. Ngày sớm nhất có thể chọn là: {minimumDate:dd/MM/yyyy}"
            });
        }

        var result = await _departureDateService.CreateDepartureDatesAsync(request, userId);
        
        if (result == null)
        {
            // Kiểm tra xem có phải lỗi khoảng cách không
            var tour = await _context.Tours
                .Include(t => t.TourOperator)
                .FirstOrDefaultAsync(t => t.TourId == request.TourId && t.IsActive);
            
            if (tour != null && tour.TourOperator.UserId == userId)
            {
                            // Kiểm tra khoảng cách với tất cả departure dates hiện có
            var existingDepartureDates = await _context.DepartureDates
                .Where(dd => dd.TourId == request.TourId && dd.IsActive)
                .ToListAsync();

            if (existingDepartureDates.Any())
            {
                var conflictingDates = new List<string>();
                
                foreach (var existingDd in existingDepartureDates)
                {
                    var daysDifference = Math.Abs((request.StartDate.Date - existingDd.DepartureDate1.Date).Days);
                    
                    if (daysDifference < 1)
                    {
                        conflictingDates.Add(existingDd.DepartureDate1.ToString("dd/MM/yyyy"));
                    }
                }
                
                if (conflictingDates.Any())
                {
                    return BadRequest(new
                    {
                        Message = $"Ngày khởi hành mới phải cách tất cả các ngày khởi hành hiện có ít nhất 1 ngày. Các ngày xung đột: {string.Join(", ", conflictingDates)}. Ngày mới phải cách mỗi ngày này ít nhất 1 ngày."
                    });
                }
            }
            }
            
            return BadRequest(new
            {
                Message = "Không thể tạo ngày khởi hành. Vui lòng kiểm tra lại thông tin tour và ngày bắt đầu hoặc bạn không có quyền tạo departure dates cho tour này."
            });
        }

        return Ok(new
        {
            Message = "Tạo ngày khởi hành thành công",
            Data = new
            {
                Id = result.Id,
                TourId = result.TourId,
                DepartureDate = result.DepartureDate1,
                IsCancelDate = result.IsCancelDate,
                IsActive = result.IsActive
            }
        });
    }

    /// <summary>
    /// Cập nhật thông tin ngày khởi hành
    /// </summary>
    /// <param name="request">Thông tin cập nhật ngày khởi hành</param>
    /// <returns>Kết quả cập nhật ngày khởi hành</returns>
    [HttpPut]
    [Authorize(Roles = Roles.TourOperator)]
    public async Task<IActionResult> UpdateDepartureDate([FromBody] UpdateDepartureDateRequest request)
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

        var validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Message = "Dữ liệu không hợp lệ",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        var result = await _departureDateService.UpdateDepartureDateAsync(request, userId);
        
        if (result == null)
        {
            // Kiểm tra các lý do có thể thất bại
            var departureDate = await _context.DepartureDates
                .Include(dd => dd.Tour)
                    .ThenInclude(t => t.TourOperator)
                .FirstOrDefaultAsync(dd => dd.Id == request.Id && dd.IsActive);
            
            if (departureDate == null)
            {
                return NotFound(new
                {
                    Message = "Không tìm thấy ngày khởi hành hoặc ngày khởi hành đã bị hủy"
                });
            }
            
            if (departureDate.Tour.TourOperator.UserId != userId)
            {
                return Forbid("Bạn không có quyền cập nhật ngày khởi hành này");
            }

            // Kiểm tra có booking confirmed không
            var hasConfirmedBookings = await _context.Bookings
                .AnyAsync(b => b.DepartureDateId == request.Id && 
                              b.IsActive && 
                              (b.BookingStatus == StatusConstants.Booking.Confirmed || 
                               b.BookingStatus == StatusConstants.Booking.Completed));

            if (hasConfirmedBookings)
            {
                return BadRequest(new
                {
                    Message = "Không thể cập nhật ngày khởi hành vì đã có booking được xác nhận"
                });
            }
            
            // Kiểm tra khoảng cách với các departure dates khác
            var otherDepartureDates = await _context.DepartureDates
                .Where(dd => dd.TourId == departureDate.TourId && 
                            dd.IsActive && 
                            dd.Id != request.Id)
                .ToListAsync();

            if (otherDepartureDates.Any())
            {
                var conflictingDates = new List<string>();
                
                foreach (var otherDd in otherDepartureDates)
                {
                    var daysDifference = Math.Abs((request.DepartureDate1.Date - otherDd.DepartureDate1.Date).Days);
                    if (daysDifference < 1)
                    {
                        conflictingDates.Add(otherDd.DepartureDate1.ToString("dd/MM/yyyy"));
                    }
                }
                
                if (conflictingDates.Any())
                {
                    return BadRequest(new
                    {
                        Message = $"Không thể cập nhật ngày khởi hành. Ngày mới phải cách tất cả các ngày khởi hành khác ít nhất 1 ngày. Các ngày xung đột: {string.Join(", ", conflictingDates)}."
                    });
                }
            }
            
            return BadRequest(new
            {
                Message = "Không thể cập nhật ngày khởi hành. Vui lòng kiểm tra lại thông tin."
            });
        }

        return Ok(new
        {
            Message = "Cập nhật ngày khởi hành thành công",
            Data = new
            {
                Id = result.Id,
                TourId = result.TourId,
                DepartureDate = result.DepartureDate1,
                IsCancelDate = result.IsCancelDate,
                IsActive = result.IsActive
            }
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