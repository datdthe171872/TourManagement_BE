using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.PurchaseServicePackage;
using TourManagement_BE.Service.PurchasedServicePackageService;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasedServicePackagesController : ControllerBase
    {
        private readonly IPurchasedServicePackageService _service;

        public PurchasedServicePackagesController(IPurchasedServicePackageService service)
        {
            _service = service;
        }

        [HttpPost("PurchaseServicePackages")]
        public async Task<IActionResult> PurchaseServicePackages([FromBody] PurchaseServicePackagesRequest request)
        {
            try
            {
                var result = await _service.PurchaseServicePackageAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("payment-webhook")]
        public async Task<IActionResult> PaymentWebhook([FromBody] PaymentNotification payload)
        {
            try
            {
                var result = await _service.ProcessPaymentWebhookAsync(payload);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
