using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data.DTO.Request.ServicePackageRequest;
using TourManagement_BE.Service.ServicePackageService;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var result = await _service.ListAllForAdminAsync();
            return Ok(result);
        }

        [HttpGet("ListAllServicePackagePaggingForAdmin")]
        public async Task<IActionResult> ListAllServicePackagePaggingForAdmin(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _service.ListAllPaginatedForAdminAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("ListAllServicePackageForCustomer")]
        public async Task<IActionResult> ListAllServicePackageForCustomer()
        {
            var result = await _service.ListAllForCustomerAsync();
            return Ok(result);
        }

        [HttpGet("ListAllServicePackagePaggingForCustomer")]
        public async Task<IActionResult> ListAllServicePackagePagging(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _service.ListAllPaginatedForCustomerAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPost("CreateServicePackage")]
        public async Task<IActionResult> CreateServicePackage([FromBody] CreateServicePackageRequest request)
        {
            await _service.CreateAsync(request);
            return Ok(new { message = "Create Service Package successfully." });
        }

        [HttpPost("AddServicePackageFeature")]
        public async Task<IActionResult> AddServicePackageFeature([FromBody] AddServicePackageFeatureRequest request)
        {
            await _service.AddFeatureAsync(request);
            return Ok(new { message = "ServicePackageFeature added successfully" });
        }

        [HttpPut("UpdateServicePackage")]
        public async Task<IActionResult> UpdateServicePackage([FromBody] UpdateServicePackageRequest request)
        {
            await _service.UpdateAsync(request);
            return Ok(new { message = "Service package updated successfully." });
        }

        [HttpPut("UpdateServicePackageFeature")]
        public async Task<IActionResult> UpdateServicePackageFeature([FromBody] UpdateServicePackageFeatureRequest request)
        {
            await _service.UpdateFeatureAsync(request);
            return Ok(new { message = "Feature updated successfully" });
        }

        [HttpPatch("ToggleServicePackageStatus/{packageId}")]
        public async Task<IActionResult> ToggleServicePackageStatus(int packageId)
        {
            await _service.ToggleStatusAsync(packageId);
            return Ok(new { message = "Service package status toggled successfully" });
        }

        [HttpPatch("ToggleServicePackageFeatureStatus/{featureid}")]
        public async Task<IActionResult> ToggleServicePackageFeatureStatus(int featureid)
        {
            await _service.ToggleFeatureStatusAsync(featureid);
            return Ok(new { message = "Service package feature status toggled successfully" });
        }

        [HttpGet("ViewDetailPackageServiceForCustomer/{packageid}")]
        public async Task<IActionResult> ViewDetailPackageServicForCustomere(int packageid)
        {
            var result = await _service.GetDetailForCustomerAsync(packageid);
            return Ok(result);
        }

        [HttpGet("ViewDetailPackageServiceForAdmin/{packageid}")]
        public async Task<IActionResult> ViewDetailPackageServiceForAdmin(int packageid)
        {
            var result = await _service.GetDetailForAdminAsync(packageid);
            return Ok(result);
        }

        [HttpGet("CheckSlotTourOperatorPackageService/{userid}")]
        public async Task<IActionResult> CheckSlotTourOperatorPackageService(int userid)
        {
            var result = await _service.CheckSlotForTourOperatorAsync(userid);
            return Ok(result);
        }
    }
}
