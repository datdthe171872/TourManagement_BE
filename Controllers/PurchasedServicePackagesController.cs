using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.DTO.Request.PurchaseServicePackage;
using TourManagement_BE.Data.DTO.Request.TourRequest;
using TourManagement_BE.Data.DTO.Response.PaymentResponse;
using TourManagement_BE.Models;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasedServicePackagesController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;

        public PurchasedServicePackagesController(MyDBContext context, IMapper mapper)
        {
            this.context = context;
            _mapper = mapper;
        }

        [HttpPost("PurchaseServicePackages")]
        public async Task<IActionResult> PurchaseServicePackages([FromBody] PurchaseServicePackagesRequest request)
        {
            var tourOperatorExists = await context.TourOperators.AnyAsync(to => to.TourOperatorId == request.TourOperatorId);
            if (!tourOperatorExists)
                return BadRequest("TourOperator không tồn tại");

            var servicePackage = await context.ServicePackages.FindAsync(request.PackageId);
            if (servicePackage == null) return NotFound("Service package not found");

            var purchaseTransaction = context.PurchaseTransactions.Add(new()
            {
                TourOperatorId = request.TourOperatorId,
                PackageId = request.PackageId,
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = request.PaymentStatus,
                ContentCode = request.ContentCode,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                IsActive = false,
                PurchasedServicePackages = new List<PurchasedServicePackage> { new()
                {
                    TourOperatorId = request.TourOperatorId,
                    PackageId = request.PackageId,
                    ActivationDate = DateTime.UtcNow.AddHours(7),
                    EndDate = DateTime.UtcNow.AddHours(7).AddYears(servicePackage.DurationInYears ?? 1),
                    NumOfToursUsed = 0,
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow.AddHours(7)
                }}
            }).Entity;

            await context.SaveChangesAsync();
            return Ok(new { message = "Purchase successful", transactionId = purchaseTransaction.TransactionId });
        }

        [HttpPost("payment-webhook")]
        public async Task<IActionResult> PaymentWebhook([FromBody] PaymentNotification payload)
        {

            // Kiểm tra thông tin thanh toán hợp lệ
            var contentCode = payload.ContentCode; 
            var amount = payload.Amount;

            // Tìm gói dịch vụ tương ứng qua contentCode
            var transaction = await context.PurchaseTransactions
                .Include(x => x.PurchasedServicePackages)
                .FirstOrDefaultAsync(x => x.PaymentStatus == "Pending" && x.ContentCode == contentCode && x.Amount == amount);

            if (transaction == null)
                return NotFound("Không tìm thấy giao dịch");

            // Đánh dấu thanh toán thành công
            transaction.PaymentStatus = "Success";
            transaction.CreatedAt = DateTime.UtcNow.AddHours(7);
            transaction.IsActive = true;

            // Kích hoạt gói dịch vụ
            var package = await context.ServicePackages.FindAsync(transaction.PackageId);
            transaction.PurchasedServicePackages.First().ActivationDate = DateTime.UtcNow.AddHours(7);
            transaction.PurchasedServicePackages.First().EndDate = DateTime.UtcNow.AddHours(7).AddYears(package.DurationInYears ?? 1);
            transaction.PurchasedServicePackages.First().IsActive = true;

            await context.SaveChangesAsync();

            return Ok(new { message = "Thanh toán thành công và đã kích hoạt gói" });
        }

    }
}
