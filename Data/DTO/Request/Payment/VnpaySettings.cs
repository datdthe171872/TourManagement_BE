namespace TourManagement_BE.Data.DTO.Request.Payment
{
    public class VnpaySettings
    {
        public string TmnCode { get; set; }         // Mã Website VNPAY cấp
        public string HashSecret { get; set; }      // Chuỗi bí mật để ký
        public string VnpUrl { get; set; }          // https://sandbox.vnpayment.vn/paymentv2/vpcpay.html
        public string ReturnUrl { get; set; }       // https://yourdomain.com/vnpay-return
    }

}
