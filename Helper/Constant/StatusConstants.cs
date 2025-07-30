namespace TourManagement_BE.Helper.Constant
{
    /// <summary>
    /// Tổng hợp tất cả các constant cho status trong hệ thống
    /// </summary>
    public static class StatusConstants
    {
        /// <summary>
        /// Payment Status Constants
        /// </summary>
        public static class Payment
        {
            public const string Pending = "Pending";
            public const string Paid = "Paid";
            public const string Failed = "Failed";
            public const string Cancelled = "Cancelled";
            public const string Refunded = "Refunded";

            public static readonly string[] ValidStatuses = { Pending, Paid, Failed, Cancelled, Refunded };
        }

        /// <summary>
        /// Booking Status Constants
        /// </summary>
        public static class Booking
        {
            public const string Pending = "Pending";
            public const string Confirmed = "Confirmed";
            public const string InProgress = "In Progress";
            public const string Completed = "Completed";
            public const string Cancelled = "Cancelled";
            public const string Rejected = "Rejected";

            public static readonly string[] ValidStatuses = { Pending, Confirmed, InProgress, Completed, Cancelled, Rejected };
        }

        /// <summary>
        /// Role Constants
        /// </summary>
        public static class Roles
        {
            public const string Customer = "Customer";
            public const string Admin = "Admin";
            public const string TourOperator = "Tour Operator";
            public const string TourGuide = "Tour Guide";

            public static readonly string[] AllRoles = { Customer, Admin, TourOperator, TourGuide };
        }
    }
} 