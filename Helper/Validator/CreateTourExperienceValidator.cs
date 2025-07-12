using FluentValidation;
using TourManagement_BE.Data.DTO.Request.TourExperienceRequest;

namespace TourManagement_BE.Helper.Validator
{
    public class CreateTourExperienceValidator : AbstractValidator<CreateTourExperience>
    {
        public CreateTourExperienceValidator()
        {
            RuleFor(x => x.TourId)
                .GreaterThan(0)
                .WithMessage("TourId phải lớn hơn 0.");

            RuleFor(x => x.Content)
                .MaximumLength(1000)
                .WithMessage("Nội dung không được vượt quá 1000 ký tự.")
                .When(x => !string.IsNullOrWhiteSpace(x.Content));
        }
    }

}
