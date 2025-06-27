using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Response.AdminDashBoard;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashBoardController : Controller
    {
        private readonly MyDBContext context;
        public AdminDashBoardController(MyDBContext context)
        {
            this.context = context;

        }

        [HttpGet("ServicePackageStatistics")]
        public IActionResult GetServicePackageStatistics()
        {
            var data = context.PurchasedServicePackages
                .Include(p => p.Package)
                .Where(p => p.IsActive && p.CreatedAt != null)
                .AsEnumerable() 
                .GroupBy(p => new { Year = p.CreatedAt.Value.Year, Month = p.CreatedAt.Value.Month })
                .Select(g => new ServicePackageStatisticsResponse
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}", 
                    PurchaseCount = g.Count(),
                    TotalRevenue = g.Sum(x => x.Package.Price)
                })
                .OrderBy(x => x.Month)
                .ToList();

            return Ok(data);
        }


    }
}
