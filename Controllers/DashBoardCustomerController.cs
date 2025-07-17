using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TourManagement_BE.Service;

namespace TourManagement_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class DashBoardCustomerController : ControllerBase
    {
        private readonly IDashboardCustomerService _dashboardService;
        public DashBoardCustomerController(IDashboardCustomerService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("total-bookings")]
        public async Task<IActionResult> GetTotalBookings()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var total = await _dashboardService.GetTotalBookingsAsync(userId);
            return Ok(total);
        }

        [HttpGet("total-transactions")]
        public async Task<IActionResult> GetTotalTransactions()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var total = await _dashboardService.GetTotalTransactionsAsync(userId);
            return Ok(total);
        }

        [HttpGet("average-value")]
        public async Task<IActionResult> GetAverageValue()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var avg = await _dashboardService.GetAverageValueAsync(userId);
            return Ok(avg);
        }

        [HttpGet("recent-bookings")]
        public async Task<IActionResult> GetRecentBookings([FromQuery] int count = 5)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var list = await _dashboardService.GetRecentBookingsAsync(userId, count);
            return Ok(list);
        }

        [HttpGet("recent-invoices")]
        public async Task<IActionResult> GetRecentInvoices([FromQuery] int count = 5)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var list = await _dashboardService.GetRecentInvoicesAsync(userId, count);
            return Ok(list);
        }
    }
} 