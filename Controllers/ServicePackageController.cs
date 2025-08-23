using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data.DTO.Request.ServicePackageRequest;
using TourManagement_BE.Helper.Constant;
using TourManagement_BE.Service.ServicePackageService;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServicePackageController : ControllerBase
    {
        private readonly IServicePackageService _service;

        public ServicePackageController(IServicePackageService service)
        {
            _service = service;
        }

        [HttpGet("ListAllServicePackageForAdmin")]
        public async Task<IActionResult> ListAllServicePackageForAdmin()
        {
            try
            {
                var result = await _service.ListAllForAdminAsync();
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ListAllServicePackagePaggingForAdmin")]
        public async Task<IActionResult> ListAllServicePackagePaggingForAdmin(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _service.ListAllPaginatedForAdminAsync(pageNumber, pageSize);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ListAllServicePackageForCustomer")]
        public async Task<IActionResult> ListAllServicePackageForCustomer()
        {
            try
            {
                var result = await _service.ListAllForCustomerAsync();
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ListAllServicePackagePaggingForCustomer")]
        public async Task<IActionResult> ListAllServicePackagePagging(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _service.ListAllPaginatedForCustomerAsync(pageNumber, pageSize);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("CreateServicePackage")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateServicePackage([FromBody] CreateServicePackageRequest request)
        {
            try
            {
                await _service.CreateAsync(request);
                return Ok(new { success = true, message = "Create Service Package successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("AddServicePackageFeature")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> AddServicePackageFeature([FromBody] AddServicePackageFeatureRequest request)
        {
            try
            {
                await _service.AddFeatureAsync(request);
                return Ok(new { success = true, message = "ServicePackageFeature added successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("UpdateServicePackage")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UpdateServicePackage([FromBody] UpdateServicePackageRequest request)
        {
            try
            {
                await _service.UpdateAsync(request);
                return Ok(new { success = true, message = "Service package updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("UpdateServicePackageFeature")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UpdateServicePackageFeature([FromBody] UpdateServicePackageFeatureRequest request)
        {
            try
            {
                await _service.UpdateFeatureAsync(request);
                return Ok(new { success = true, message = "Feature updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPatch("ToggleServicePackageStatus/{packageId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ToggleServicePackageStatus(int packageId)
        {
            try
            {
                await _service.ToggleStatusAsync(packageId);
                return Ok(new { success = true, message = "Service package status toggled successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPatch("ToggleServicePackageFeatureStatus/{featureid}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ToggleServicePackageFeatureStatus(int featureid)
        {
            try
            {
                await _service.ToggleFeatureStatusAsync(featureid);
                return Ok(new { success = true, message = "Service package feature status toggled successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ViewDetailPackageServiceForCustomer/{packageid}")]
        public async Task<IActionResult> ViewDetailPackageServicForCustomere(int packageid)
        {
            try
            {
                var result = await _service.GetDetailForCustomerAsync(packageid);
                return Ok(new { success = true, data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("ViewDetailPackageServiceForAdmin/{packageid}")]
        public async Task<IActionResult> ViewDetailPackageServiceForAdmin(int packageid)
        {
            try
            {
                var result = await _service.GetDetailForAdminAsync(packageid);
                return Ok(new { success = true, data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("CheckSlotTourOperatorPackageService/{userid}")]
        public async Task<IActionResult> CheckSlotTourOperatorPackageService(int userid)
        {
            var result = await _service.CheckSlotForTourOperatorAsync(userid);
            return Ok(result);
        }
    }
}
