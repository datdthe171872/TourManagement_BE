using System;

namespace TourManagement_BE.Helper.Exceptions
{
    public class BookingException : Exception
    {
        public string ErrorCode { get; }

        public BookingException(string message, string errorCode = "BOOKING_ERROR") : base(message)
        {
            ErrorCode = errorCode;
        }

        public BookingException(string message, Exception innerException, string errorCode = "BOOKING_ERROR") : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }

    public class PaymentDueDateException : BookingException
    {
        public PaymentDueDateException(string message) : base(message, "PAYMENT_DUE_DATE_ERROR")
        {
        }
    }

    public class InsufficientSlotsException : BookingException
    {
        public InsufficientSlotsException(string message) : base(message, "INSUFFICIENT_SLOTS_ERROR")
        {
        }
    }

    public class TourNotFoundException : BookingException
    {
        public TourNotFoundException(string message) : base(message, "TOUR_NOT_FOUND_ERROR")
        {
        }
    }

    public class DepartureDateNotFoundException : BookingException
    {
        public DepartureDateNotFoundException(string message) : base(message, "DEPARTURE_DATE_NOT_FOUND_ERROR")
        {
        }
    }
}
