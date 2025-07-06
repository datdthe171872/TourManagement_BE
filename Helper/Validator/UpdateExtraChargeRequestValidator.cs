using FluentValidation;
using TourManagement_BE.Data.DTO.Request;

namespace TourManagement_BE.Helper.Validator
{
    public class UpdateExtraChargeRequestValidator : AbstractValidator<UpdateExtraChargeRequest>
    {
        public UpdateExtraChargeRequestValidator()
        {
            RuleFor(x => x.ExtraChargeId).GreaterThan(0);
        }
    }
} 