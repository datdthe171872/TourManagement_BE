using FluentValidation;
using TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Create;

namespace TourManagement_BE.Helper.Validator
{
    public class TourExperienceCreateDtoValidator : AbstractValidator<TourExperienceCreateDto>
    {
        public TourExperienceCreateDtoValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Nội dung trải nghiệm là bắt buộc.")
                .MaximumLength(1000).WithMessage("Nội dung trải nghiệm không được vượt quá 1000 ký tự.");
        }
    }
}
