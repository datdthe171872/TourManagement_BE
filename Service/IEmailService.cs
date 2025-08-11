using System;
using System.Threading.Tasks;

namespace TourManagement_BE.Service
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendPaymentDueEmailAsync(string toEmail, string customerName, int bookingId, DateTime paymentDueDate, decimal totalAmount);
        Task SendPaymentOverdueEmailAsync(string toEmail, string customerName, int bookingId, DateTime paymentDueDate, decimal totalAmount);
        Task SendBookingCancelledEmailAsync(string toEmail, string customerName, int bookingId, string reason);
        Task SendRefundEmailAsync(string toEmail, string customerName, int bookingId, decimal refundAmount, string refundPercentage);
        Task SendBookingUpdateEmailAsync(string toEmail, string customerName, int bookingId, string updateType);
        Task SendTourOperatorNotificationEmailAsync(string toEmail, string tourOperatorName, int bookingId, string title, string message);
    }
}
