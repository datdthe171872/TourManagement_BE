using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TourManagement_BE.Data.DTO.Request.Payment;
using TourManagement_BE.Helper.Common;
using TourManagement_BE.Models;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentProcessingController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PaymentProcessingController(MyDBContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost("confirm-vnpay-payment")]
        public IActionResult ConfirmVnpayPayment([FromBody] VnpayPaymentCallbackRequest request)
        {
            var payment = context.Payments
                .FirstOrDefault(p => p.BookingId == request.BookingId && p.PaymentStatus == "Pending");

            if (payment == null)
            {
                return NotFound(new { Message = "Không tìm thấy thanh toán đang chờ xử lý." });
            }

            // Cập nhật thông tin thanh toán
            payment.AmountPaid = request.AmountPaid;
            payment.PaymentMethod = request.PaymentMethod;
            payment.PaymentStatus = "Completed";
            payment.PaymentDate = DateTime.Now;
            payment.PaymentReference = request.PaymentReference;
            payment.IsActive = true;

            // Cập nhật trạng thái Booking nếu muốn
            var booking = context.Bookings.FirstOrDefault(b => b.BookingId == request.BookingId);
            if (booking != null)
            {
                booking.PaymentStatus = "Paid";
            }

            context.SaveChanges();

            return Ok(new { Message = "Xác nhận thanh toán thành công." });
        }

/*        [HttpGet("CreatePaymentUrl")]
        public IActionResult CreatePaymentUrl(int bookingId)
        {
            var vnpay = new VnPayLibrary();

            var timeNow = DateTime.Now;
            var txnRef = DateTime.Now.Ticks.ToString();
            var payment = context.Payments.FirstOrDefault(p => p.BookingId == bookingId && p.PaymentStatus == "Pending");
            if (payment == null) return BadRequest("Không tìm thấy thanh toán đang chờ");

            // Thêm các tham số bắt buộc
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _configuration["VnpaySettings:TmnCode"]);
            var amount = ((payment.Amount) * 100);
            vnpay.AddRequestData("vnp_Amount", amount.ToString());
            vnpay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng #" + txnRef);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VnpaySettings:ReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", txnRef);
            vnpay.AddRequestData("vnp_ExpireDate", timeNow.AddMinutes(15).ToString("yyyyMMddHHmmss"));

            string paymentUrl = vnpay.CreateRequestUrl(
                _configuration["VnpaySettings:VnpUrl"],
                _configuration["VnpaySettings:HashSecret"]
            );

            return Redirect(paymentUrl); // hoặc return Ok(new { url = paymentUrl });
        }


        [HttpGet("/vnpay-return")]
        public IActionResult VnpayReturn()
        {
            var vnpay = _configuration.GetSection("VnpaySettings").Get<VnpaySettings>();
            var vnpayLib = new VnPayLibrary();

            foreach (var key in Request.Query.Keys)
            {
                if (key.StartsWith("vnp_"))
                    vnpayLib.AddResponseData(key, Request.Query[key]);
            }

            var isValid = vnpayLib.ValidateSignature(vnpay.HashSecret);
            if (!isValid)
                return Content("Sai chữ ký!");

            var code = vnpayLib.GetResponseData("vnp_ResponseCode");
            var txnRef = vnpayLib.GetResponseData("vnp_TxnRef");

            if (code == "00")
            {
                var bookingId = int.Parse(txnRef);
                var payment = context.Payments.FirstOrDefault(p => p.BookingId == bookingId);
                if (payment != null)
                {
                    payment.PaymentStatus = "Completed";
                    payment.PaymentMethod = "VNPAY";
                    payment.IsActive = true;
                    payment.PaymentDate = DateTime.Now;
                    payment.PaymentReference = vnpayLib.GetResponseData("vnp_TransactionNo");
                    context.SaveChanges();
                }

                return Content("Thanh toán thành công!");
            }

            return Content("Thanh toán thất bại.");
        }*/

    }
}
