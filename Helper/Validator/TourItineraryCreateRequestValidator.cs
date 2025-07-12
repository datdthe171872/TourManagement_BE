using FluentValidation;
using TourManagement_BE.Data.DTO.Request.TourItineraryRequest;

namespace TourManagement_BE.Helper.Validator
{
    public class TourItineraryCreateRequestValidator : AbstractValidator<TourItineraryCreateRequest>
    {
        public TourItineraryCreateRequestValidator()
        {
            RuleFor(x => x.TourId)
                .GreaterThan(0)
                .WithMessage("TourId phải lớn hơn 0.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Tiêu đề là bắt buộc.")
                .MaximumLength(255)
                .WithMessage("Tiêu đề không được vượt quá 255 ký tự.");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Mô tả không được vượt quá 1000 ký tự.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }

}
