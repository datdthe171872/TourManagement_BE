using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Service;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly MyDBContext _context;

        public ReportController(IReportService reportService, MyDBContext context)
        {
            _reportService = reportService;
            _context = context;
        }

        // API cho Customer - Lấy báo cáo của chính mình
        [HttpGet("customer")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<List<ReportResponse>>> GetCustomerReports(
            [FromQuery] string? username = null)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var reports = await _reportService.GetReportsForCustomerAsync(userId, username);
            return Ok(reports);
        }

        // API cho Tour Operator - Lấy báo cáo của các booking thuộc tour của operator
        [HttpGet("operator")]
        [Authorize(Roles = "Tour Operator")]
        public async Task<ActionResult<List<ReportResponse>>> GetOperatorReports(
            [FromQuery] string? username = null)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            // Lấy TourOperatorId từ User
            var tourOperator = await _context.TourOperators
                .FirstOrDefaultAsync(to => to.UserId == userId && to.IsActive);
            
            if (tourOperator == null)
            {
                return NotFound("Tour Operator not found");
            }

            var reports = await _reportService.GetReportsForOperatorAsync(tourOperator.TourOperatorId, username);
            return Ok(reports);
        }
    }
} 