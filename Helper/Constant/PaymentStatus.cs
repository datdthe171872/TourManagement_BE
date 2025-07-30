namespace TourManagement_BE.Helper.Constant
{
    public static class PaymentStatus
    {
        public const string Pending = "Pending";
        public const string Paid = "Paid";
        public const string Failed = "Failed";
        public const string Cancelled = "Cancelled";
        public const string Refunded = "Refunded";

        public static readonly string[] ValidStatuses = { Pending, Paid, Failed, Cancelled, Refunded };
    }
} 