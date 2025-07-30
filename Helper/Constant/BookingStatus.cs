namespace TourManagement_BE.Helper.Constant
{
    public static class BookingStatus
    {
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
        public const string InProgress = "In Progress";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
        public const string Rejected = "Rejected";

        public static readonly string[] ValidStatuses = { Pending, Confirmed, InProgress, Completed, Cancelled, Rejected };
    }
} 