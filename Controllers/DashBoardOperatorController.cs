using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TourManagement_BE.Service;

namespace TourManagement_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashBoardOperatorController : ControllerBase
    {
        private readonly IDashboardOperatorService _dashboardService;
        public DashBoardOperatorController(IDashboardOperatorService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("total-tours")]
        public async Task<IActionResult> GetTotalTours()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var total = await _dashboardService.GetTotalToursAsync(userId);
            return Ok(total);
        }

        [HttpGet("total-bookings")]
        public async Task<IActionResult> GetTotalBookings()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var total = await _dashboardService.GetTotalBookingsAsync(userId);
            return Ok(total);
        }

        [HttpGet("total-earnings")]
        public async Task<IActionResult> GetTotalEarnings()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var total = await _dashboardService.GetTotalEarningsAsync(userId);
            return Ok(total);
        }

        [HttpGet("total-feedbacks")]
        public async Task<IActionResult> GetTotalFeedbacks()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var total = await _dashboardService.GetTotalFeedbacksAsync(userId);
            return Ok(total);
        }

        [HttpGet("latest-invoices")]
        public async Task<IActionResult> GetLatestInvoices([FromQuery] int count = 5)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var invoices = await _dashboardService.GetLatestInvoicesAsync(userId, count);
            return Ok(invoices);
        }
    }
} 