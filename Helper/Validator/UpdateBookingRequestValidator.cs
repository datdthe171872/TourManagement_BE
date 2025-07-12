using FluentValidation;
using TourManagement_BE.Data.DTO.Request;

namespace TourManagement_BE.Helper.Validator
{
    public class UpdateBookingRequestValidator : AbstractValidator<UpdateBookingRequest>
    {
        public UpdateBookingRequestValidator()
        {
            RuleFor(x => x.BookingId).GreaterThan(0);
        }
    }
} 