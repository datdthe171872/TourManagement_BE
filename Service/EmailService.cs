using System;
using System.Threading.Tasks;
using TourManagement_BE.Helper.Common;

namespace TourManagement_BE.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailHelper _emailHelper;

        public EmailService(EmailHelper emailHelper)
        {
            _emailHelper = emailHelper;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            await _emailHelper.SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendPaymentDueEmailAsync(string toEmail, string customerName, int bookingId, DateTime paymentDueDate, decimal totalAmount)
        {
            var subject = $"Nhắc nhở: Hạn nộp tiền cho đặt tour #{bookingId}";
            var body = $@"
                <h2>Xin chào {customerName},</h2>
                <p>Đây là email nhắc nhở về hạn nộp tiền cho đặt tour của bạn.</p>
                <p><strong>Mã đặt tour:</strong> #{bookingId}</p>
                <p><strong>Hạn nộp tiền:</strong> {paymentDueDate:dd/MM/yyyy}</p>
                <p><strong>Số tiền cần nộp:</strong> {totalAmount:N0} VNĐ</p>
                <p>Vui lòng hoàn tất thanh toán trước hạn để tránh việc hủy đặt tour tự động.</p>
                <p>Trân trọng,<br>Đội ngũ hỗ trợ</p>";

            await _emailHelper.SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendPaymentOverdueEmailAsync(string toEmail, string customerName, int bookingId, DateTime paymentDueDate, decimal totalAmount)
        {
            var subject = $"CẢNH BÁO: Quá hạn nộp tiền cho đặt tour #{bookingId}";
            var body = $@"
                <h2>Xin chào {customerName},</h2>
                <p>Đặt tour của bạn đã quá hạn nộp tiền và sẽ bị hủy tự động.</p>
                <p><strong>Mã đặt tour:</strong> #{bookingId}</p>
                <p><strong>Hạn nộp tiền:</strong> {paymentDueDate:dd/MM/yyyy}</p>
                <p><strong>Số tiền cần nộp:</strong> {totalAmount:N0} VNĐ</p>
                <p>Đặt tour sẽ bị hủy trong thời gian ngắn. Nếu bạn vẫn muốn tham gia tour, vui lòng liên hệ ngay với chúng tôi.</p>
                <p>Trân trọng,<br>Đội ngũ hỗ trợ</p>";

            await _emailHelper.SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendBookingCancelledEmailAsync(string toEmail, string customerName, int bookingId, string reason)
        {
            var subject = $"Thông báo: Đặt tour #{bookingId} đã bị hủy";
            var body = $@"
                <h2>Xin chào {customerName},</h2>
                <p>Đặt tour của bạn đã bị hủy.</p>
                <p><strong>Mã đặt tour:</strong> #{bookingId}</p>
                <p><strong>Lý do hủy:</strong> {reason}</p>
                <p>Nếu bạn có thắc mắc, vui lòng liên hệ với chúng tôi.</p>
                <p>Trân trọng,<br>Đội ngũ hỗ trợ</p>";

            await _emailHelper.SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendRefundEmailAsync(string toEmail, string customerName, int bookingId, decimal refundAmount, string refundPercentage)
        {
            var subject = $"Thông báo: Hoàn tiền cho đặt tour #{bookingId}";
            var body = $@"
                <h2>Xin chào {customerName},</h2>
                <p>Đặt tour của bạn đã được hủy và tiền hoàn sẽ được xử lý.</p>
                <p><strong>Mã đặt tour:</strong> #{bookingId}</p>
                <p><strong>Tỷ lệ hoàn tiền:</strong> {refundPercentage}</p>
                <p><strong>Số tiền hoàn:</strong> {refundAmount:N0} VNĐ</p>
                <p>Tiền hoàn sẽ được chuyển về tài khoản của bạn trong vòng 3-5 ngày làm việc.</p>
                <p>Trân trọng,<br>Đội ngũ hỗ trợ</p>";

            await _emailHelper.SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendBookingUpdateEmailAsync(string toEmail, string customerName, int bookingId, string updateType)
        {
            var subject = $"Thông báo: Cập nhật đặt tour #{bookingId}";
            var body = $@"
                <h2>Xin chào {customerName},</h2>
                <p>Đặt tour của bạn đã được cập nhật.</p>
                <p><strong>Mã đặt tour:</strong> #{bookingId}</p>
                <p><strong>Loại cập nhật:</strong> {updateType}</p>
                <p>Vui lòng kiểm tra thông tin mới trong tài khoản của bạn.</p>
                <p>Trân trọng,<br>Đội ngũ hỗ trợ</p>";

            await _emailHelper.SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendTourOperatorNotificationEmailAsync(string toEmail, string tourOperatorName, int bookingId, string title, string message)
        {
            var subject = $"Thông báo: {title} - Đặt tour #{bookingId}";
            var body = $@"
                <h2>Xin chào {tourOperatorName},</h2>
                <p>{message}</p>
                <p><strong>Mã đặt tour:</strong> #{bookingId}</p>
                <p>Vui lòng kiểm tra và xử lý theo yêu cầu.</p>
                <p>Trân trọng,<br>Hệ thống quản lý tour</p>";

            await _emailHelper.SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendBookingCreatedPaymentEmailAsync(string toEmail, string customerName, int bookingId, decimal totalAmount, DateTime paymentDeadline, string tourOperatorPhone)
        {
            var subject = $"Thông báo: Đặt tour #{bookingId} thành công - Thông tin thanh toán";
            var body = $@"
                <h2>Xin chào {customerName},</h2>
                <p>Chúc mừng! Đặt tour của bạn đã được tạo thành công.</p>
                <p><strong>Mã đặt tour:</strong> #{bookingId}</p>
                <p><strong>Số tiền cần thanh toán:</strong> {totalAmount:N0} VNĐ</p>
                <p><strong>Hạn thanh toán:</strong> {paymentDeadline:dd/MM/yyyy}</p>
                <p>Vui lòng hoàn tất thanh toán trước hạn để đảm bảo vị trí của bạn trong tour.</p>
                <p>Nếu bạn có thắc mắc, vui lòng liên hệ qua số điện thoại: <strong>{tourOperatorPhone}</strong></p>
                <p>Trân trọng,<br>Đội ngũ hỗ trợ</p>";

            await _emailHelper.SendEmailAsync(toEmail, subject, body);
        }
    }
}
//abc