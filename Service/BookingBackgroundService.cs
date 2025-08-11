using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Helper.Constant;
using TourManagement_BE.Service;

namespace TourManagement_BE.BackgroundServices
{
    public class BookingBackgroundService : BackgroundService
    {
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Kiểm tra mỗi giờ
        private readonly IServiceProvider _serviceProvider;

        public BookingBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine($"[BookingBackground] ✅ Bắt đầu kiểm tra booking quá hạn lúc {DateTime.Now}");

                    await CheckAndCancelOverdueBookingsAsync();

                    Console.WriteLine($"[BookingBackground] ✅ Hoàn tất kiểm tra booking quá hạn lúc {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[BookingBackground] ❌ Lỗi khi kiểm tra booking quá hạn: {ex.Message}");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckAndCancelOverdueBookingsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MyDBContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var currentDate = DateTime.Now;

            // Lấy tất cả booking có PaymentStatus = Pending và đã quá hạn nộp tiền
            var overdueBookings = await dbContext.Bookings
                .Include(b => b.DepartureDate)
                .Include(b => b.User)
                .Include(b => b.Tour)
                .ThenInclude(t => t.TourOperator)
                .ThenInclude(to => to.User)
                .Where(b => b.IsActive && 
                           b.PaymentStatus == PaymentStatus.Pending &&
                           b.BookingStatus != BookingStatus.Cancelled &&
                           b.DepartureDate.DepartureDate1 > currentDate) // Chỉ xử lý tour chưa đi
                .ToListAsync();

            foreach (var booking in overdueBookings)
            {
                var paymentDueDate = booking.DepartureDate.DepartureDate1.AddDays(-21); // 3 tuần trước ngày đi

                if (currentDate > paymentDueDate)
                {
                    try
                    {
                        // Cập nhật trạng thái booking thành Cancelled
                        booking.BookingStatus = BookingStatus.Cancelled;
                        booking.PaymentStatus = PaymentStatus.Cancelled;
                        booking.IsActive = false;

                        // Gửi thông báo cho khách hàng
                        await notificationService.CreatePaymentOverdueNotificationAsync(
                            booking.UserId, 
                            booking.BookingId, 
                            paymentDueDate);

                        // Gửi email cho khách hàng
                        if (!string.IsNullOrEmpty(booking.User.Email))
                        {
                            await emailService.SendPaymentOverdueEmailAsync(
                                booking.User.Email,
                                booking.User.UserName ?? "Khách hàng",
                                booking.BookingId,
                                paymentDueDate,
                                booking.TotalPrice ?? 0);
                        }

                        // Gửi thông báo cho Tour Operator
                        if (booking.Tour?.TourOperator?.UserId != null)
                        {
                            await notificationService.CreateTourOperatorNotificationAsync(
                                booking.Tour.TourOperator.UserId.Value,
                                booking.BookingId,
                                "Booking bị hủy tự động",
                                $"Booking #{booking.BookingId} đã bị hủy tự động do quá hạn nộp tiền");

                            if (!string.IsNullOrEmpty(booking.Tour.TourOperator.User.Email))
                            {
                                await emailService.SendTourOperatorNotificationEmailAsync(
                                    booking.Tour.TourOperator.User.Email,
                                    booking.Tour.TourOperator.User.UserName ?? "Tour Operator",
                                    booking.BookingId,
                                    "Booking bị hủy tự động",
                                    $"Booking #{booking.BookingId} đã bị hủy tự động do quá hạn nộp tiền");
                            }
                        }

                        Console.WriteLine($"[BookingBackground] ✅ Đã hủy booking #{booking.BookingId} do quá hạn nộp tiền");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[BookingBackground] ❌ Lỗi khi hủy booking #{booking.BookingId}: {ex.Message}");
                    }
                }
            }

            // Lưu thay đổi vào database
            if (overdueBookings.Any())
            {
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
