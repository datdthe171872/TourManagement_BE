using FluentValidation;
using TourManagement_BE.Data.DTO.Request;

namespace TourManagement_BE.Helper.Validator
{
    public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
    {
        public CreateBookingRequestValidator()
        {
            RuleFor(x => x.TourId).GreaterThan(0);
        }
    }
} 