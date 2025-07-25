using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.PurchaseServicePackage;
using TourManagement_BE.Data.DTO.Response.PaymentResponse;
using TourManagement_BE.Data.Models;

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

            var timeNow = DateTime.UtcNow.AddHours(7);
            var endDate = timeNow.AddYears(request.NumberYearActive);
            var contentCode = GenerateContentCode();

            var purchaseTransaction = context.PurchaseTransactions.Add(new()
            {
                TourOperatorId = request.TourOperatorId,
                PackageId = request.PackageId,
                Amount = request.Amount * request.NumberYearActive,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = "Pending",
                ContentCode = contentCode,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                IsActive = false,
                PurchasedServicePackages = new List<PurchasedServicePackage> { new()
                {
                    TourOperatorId = request.TourOperatorId,
                    PackageId = request.PackageId,
                    ActivationDate = timeNow,
                    EndDate = endDate,
                    NumOfToursUsed = 0,
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow.AddHours(7)
                }}
            }).Entity;

            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Purchase successful",
                transactionId = purchaseTransaction.TransactionId,
                contentCode = contentCode
            });
        }

        [HttpPost("payment-webhook")]
        public async Task<IActionResult> PaymentWebhook([FromBody] PaymentNotification payload)
        {
            var contentCode = payload.ContentCode;
            var amount = payload.Amount;

            var transaction = await context.PurchaseTransactions
                .Include(x => x.PurchasedServicePackages)
                .FirstOrDefaultAsync(x => x.PaymentStatus == "Pending" && x.ContentCode == contentCode && x.Amount == amount);

            if (transaction == null)
                return NotFound("Không tìm thấy giao dịch");

            transaction.PaymentStatus = "Completed";
            transaction.CreatedAt = DateTime.UtcNow.AddHours(7);
            transaction.IsActive = true;    

            transaction.PurchasedServicePackages.First().IsActive = true;

            await context.SaveChangesAsync();

            return Ok(new { message = "Thanh toán thành công và đã kích hoạt gói" });
        }

        private static readonly Random random = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string numbers = "0123456789";

        public static string GenerateContentCode()
        {
            const string prefix = "PKG"; 
            const int suffixLength = 5;

            var suffix = new char[suffixLength];
            suffix[0] = numbers[random.Next(numbers.Length)];

            var allChars = chars + numbers;
            for (int i = 1; i < suffixLength; i++)
            {
                suffix[i] = allChars[random.Next(allChars.Length)];
            }

            var shuffledSuffix = suffix.Skip(1).OrderBy(_ => random.Next()).ToArray();

            return prefix + suffix[0] + new string(shuffledSuffix);
        }
    }
}
