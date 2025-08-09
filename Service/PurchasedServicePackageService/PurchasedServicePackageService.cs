using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.PurchaseServicePackage;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Service.PurchasedServicePackageService;

namespace TourManagement_BE.Service.PurchasedServicePackageService
{
    public class PurchasedServicePackageService : IPurchasedServicePackageService
    {
        private readonly MyDBContext _context;
        private readonly Random _random = new Random();
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _numbers = "0123456789";

        public PurchasedServicePackageService(MyDBContext context)
        {
            _context = context;
        }

        /*public async Task<PurchaseResult> PurchaseServicePackageAsync(PurchaseServicePackagesRequest request)
        {
            var tourOperator = await _context.TourOperators
                .FirstOrDefaultAsync(to => to.UserId == request.UserId)
                ?? throw new InvalidOperationException("TourOperator không tồn tại");

            var servicePackage = await _context.ServicePackages
                .FindAsync(request.PackageId)
                ?? throw new KeyNotFoundException("Service package not found");

            var timeNow = DateTime.UtcNow.AddHours(7);
            var endDate = timeNow.AddYears(request.NumberYearActive);
            var contentCode = GenerateContentCode();

            var purchaseTransaction = new PurchaseTransaction
            {
                TourOperatorId = tourOperator.TourOperatorId,
                PackageId = request.PackageId,
                Amount = request.Amount * request.NumberYearActive,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = "Pending",
                ContentCode = contentCode,
                CreatedAt = timeNow,
                IsActive = false,
                PurchasedServicePackages = new List<PurchasedServicePackage>
                {
                    new PurchasedServicePackage
                    {
                        TourOperatorId = tourOperator.TourOperatorId,
                        PackageId = request.PackageId,
                        ActivationDate = timeNow,
                        EndDate = endDate,
                        NumOfToursUsed = 0,
                        IsActive = false,
                        CreatedAt = timeNow
                    }
                }
            };

            await _context.PurchaseTransactions.AddAsync(purchaseTransaction);
            await _context.SaveChangesAsync();

            return new PurchaseResult
            {
                Message = "Purchase successful",
                TransactionId = purchaseTransaction.TransactionId,
                ContentCode = contentCode
            };
        }*/

        public async Task<PurchaseResult> PurchaseServicePackageAsync(PurchaseServicePackagesRequest request)
        {
            var tourOperator = await _context.TourOperators
                .FirstOrDefaultAsync(to => to.UserId == request.UserId)
                ?? throw new InvalidOperationException("TourOperator không tồn tại");

            var servicePackage = await _context.ServicePackages
                .FindAsync(request.PackageId)
                ?? throw new KeyNotFoundException("Service package not found");

            var timeNow = DateTime.UtcNow.AddHours(7);

            // ❗ Kiểm tra đã mua gói này và còn hạn không
            var existingPackage = await _context.PurchasedServicePackages
                .Where(p => p.TourOperatorId == tourOperator.TourOperatorId
                         && p.PackageId == request.PackageId
                         && p.IsActive == true
                         && p.EndDate > timeNow)
                .FirstOrDefaultAsync();

            if (existingPackage != null)
            {
                return new PurchaseResult
                {
                    Message = "TourOperator đã mua gói này và vẫn còn thời gian sử dụng",
                    TransactionId = 0,
                    ContentCode = null
                };
            }

            var endDate = timeNow.AddYears(request.NumberYearActive);
            var contentCode = GenerateContentCode();

            var purchaseTransaction = new PurchaseTransaction
            {
                TourOperatorId = tourOperator.TourOperatorId,
                PackageId = request.PackageId,
                Amount = request.Amount * request.NumberYearActive,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = "Pending",
                ContentCode = contentCode,
                CreatedAt = timeNow,
                IsActive = false,
                PurchasedServicePackages = new List<PurchasedServicePackage>
        {
            new PurchasedServicePackage
            {
                TourOperatorId = tourOperator.TourOperatorId,
                PackageId = request.PackageId,
                ActivationDate = timeNow,
                EndDate = endDate,
                NumOfToursUsed = 0,
                IsActive = false,
                CreatedAt = timeNow
            }
        }
            };

            await _context.PurchaseTransactions.AddAsync(purchaseTransaction);
            await _context.SaveChangesAsync();

            return new PurchaseResult
            {
                Message = "Purchase successful",
                TransactionId = purchaseTransaction.TransactionId,
                ContentCode = contentCode
            };
        }


        public async Task<PaymentResult> ProcessPaymentWebhookAsync(PaymentNotification payload)
        {
            var transaction = await _context.PurchaseTransactions
                .Include(x => x.PurchasedServicePackages)
                .FirstOrDefaultAsync(x => x.PaymentStatus == "Pending" &&
                                        x.ContentCode == payload.ContentCode &&
                                        x.Amount == payload.Amount)
                ?? throw new KeyNotFoundException("Không tìm thấy giao dịch");

            transaction.PaymentStatus = "Completed";
            transaction.CreatedAt = DateTime.UtcNow.AddHours(7);
            transaction.IsActive = true;
            transaction.PurchasedServicePackages.First().IsActive = true;

            await _context.SaveChangesAsync();

            return new PaymentResult
            {
                Message = "Thanh toán thành công và đã kích hoạt gói"
            };
        }

        private string GenerateContentCode()
        {
            const string prefix = "PKG";
            const int suffixLength = 5;

            var suffix = new char[suffixLength];
            suffix[0] = _numbers[_random.Next(_numbers.Length)];

            var allChars = _chars + _numbers;
            for (int i = 1; i < suffixLength; i++)
            {
                suffix[i] = allChars[_random.Next(allChars.Length)];
            }

            var shuffledSuffix = suffix.Skip(1).OrderBy(_ => _random.Next()).ToArray();

            return prefix + suffix[0] + new string(shuffledSuffix);
        }
    }
}
