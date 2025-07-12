using FluentValidation;
using TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Create;

namespace TourManagement_BE.Helper.Validator
{
    public class TourItineraryCreateDtoValidator : AbstractValidator<TourItineraryCreateDto>
    {
        public TourItineraryCreateDtoValidator()
        {
            RuleFor(x => x.DayNumber)
                .GreaterThan(0).WithMessage("Ngày (DayNumber) phải lớn hơn 0.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tiêu đề lịch trình là bắt buộc.")
                .MaximumLength(255).WithMessage("Tiêu đề lịch trình không được vượt quá 255 ký tự.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Mô tả lịch trình là bắt buộc.");

            RuleForEach(x => x.ItineraryMedia)
                .SetValidator(new ItineraryMediaCreateDtoValidator());
        }
    }

}
