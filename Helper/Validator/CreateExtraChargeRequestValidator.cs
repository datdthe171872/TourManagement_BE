using FluentValidation;
using TourManagement_BE.Data.DTO.Request;

namespace TourManagement_BE.Helper.Validator
{
    public class CreateExtraChargeRequestValidator : AbstractValidator<CreateExtraChargeRequest>
    {
        public CreateExtraChargeRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
} 