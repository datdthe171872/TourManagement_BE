using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Service;

namespace TourManagement_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompareController : ControllerBase
    {
        private readonly ITourComparisonService _tourComparisonService;

        public CompareController(ITourComparisonService tourComparisonService)
        {
            _tourComparisonService = tourComparisonService;
        }

        /// <summary>
        /// So sánh 2 tour dựa trên 5 tiêu chí chính
        /// </summary>
        /// <param name="request">Tour1Id và Tour2Id</param>
        /// <returns>Kết quả so sánh chi tiết</returns>
        [HttpPost("tours")]
        public async Task<IActionResult> CompareTours([FromBody] TourComparisonRequest request)
        {
            try
            {
                if (request.Tour1Id == request.Tour2Id)
                    return BadRequest("Không thể so sánh tour với chính nó");

                if (request.Tour1Id <= 0 || request.Tour2Id <= 0)
                    return BadRequest("TourId phải là số dương");

                var result = await _tourComparisonService.CompareToursAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi khi so sánh tour: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy thông tin so sánh nhanh giữa 2 tour
        /// </summary>
        /// <param name="tour1Id">ID của tour thứ nhất</param>
        /// <param name="tour2Id">ID của tour thứ hai</param>
        /// <returns>Kết quả so sánh</returns>
        [HttpGet("tours/{tour1Id}/{tour2Id}")]
        public async Task<IActionResult> CompareToursGet(int tour1Id, int tour2Id)
        {
            try
            {
                if (tour1Id == tour2Id)
                    return BadRequest("Không thể so sánh tour với chính nó");

                if (tour1Id <= 0 || tour2Id <= 0)
                    return BadRequest("TourId phải là số dương");

                var request = new TourComparisonRequest
                {
                    Tour1Id = tour1Id,
                    Tour2Id = tour2Id
                };

                var result = await _tourComparisonService.CompareToursAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi khi so sánh tour: {ex.Message}");
            }
        }
    }
}
