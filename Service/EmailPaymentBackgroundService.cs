using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using TourManagement_BE.Service; 

namespace TourManagement_BE.BackgroundServices
{
    public class EmailPaymentBackgroundService : BackgroundService
    {
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); 

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine($"[EmailPayment] ✅ Bắt đầu kiểm tra email lúc {DateTime.Now}");

                    await PaymentIMAP.CheckEmailAndProcessPayment();

                    Console.WriteLine($"[EmailPayment] ✅ Hoàn tất kiểm tra email lúc {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EmailPayment] ❌ Lỗi khi xử lý email: {ex.Message}");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
