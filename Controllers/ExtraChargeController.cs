using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Service;

namespace TourManagement_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExtraChargeController : ControllerBase
    {
        private readonly IExtraChargeService _service;
        public ExtraChargeController(IExtraChargeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
        {
            var result = await _service.GetAllAsync(isActive);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExtraChargeRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateExtraChargeRequest request)
        {
            var result = await _service.UpdateAsync(request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPatch("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return Ok();
        }
    }
} 