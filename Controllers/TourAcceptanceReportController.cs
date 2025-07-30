using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Service;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Tour Guide")]
    public class TourAcceptanceReportController : ControllerBase
    {
        private readonly ITourAcceptanceReportService _reportService;

        public TourAcceptanceReportController(ITourAcceptanceReportService reportService)
        {
            _reportService = reportService;
        }

        // Lấy danh sách report của guide hiện tại
        [HttpGet]
        public async Task<ActionResult<List<TourAcceptanceReportResponse>>> GetMyReports()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var reports = await _reportService.GetReportsByGuideUserIdAsync(userId);
            return Ok(reports);
        }

        // Lấy chi tiết report theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<TourAcceptanceReportResponse>> GetReportById(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var report = await _reportService.GetReportByIdAsync(id, userId);
            return Ok(report);
        }

        // Cập nhật report (chỉ Notes và AttachmentUrl)
        [HttpPut("{id}")]
        public async Task<ActionResult<TourAcceptanceReportResponse>> UpdateReport(int id, [FromBody] UpdateTourAcceptanceReportRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var report = await _reportService.UpdateReportAsync(userId, id, request);
            return Ok(report);
        }

        // Lấy reports theo booking ID (có thể dùng cho admin/operator)
        [HttpGet("bookingforOperator/{bookingId}")]
        [Authorize(Roles = "Admin,Tour Operator")]
        public async Task<ActionResult<List<TourAcceptanceReportResponse>>> GetReportsByBookingId(int bookingId)
        {
            var reports = await _reportService.GetReportsByBookingIdAsync(bookingId);
            return Ok(reports);
        }
    }
} 