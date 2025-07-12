using FluentValidation;
using TourManagement_BE.Data.DTO.Request.TourRequest;

namespace TourManagement_BE.Helper.Validator
{
    public class TourCreateRequestValidator : AbstractValidator<TourCreateRequest>
    {
        public TourCreateRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tiêu đề tour là bắt buộc.")
                .MaximumLength(255).WithMessage("Tiêu đề tour không được vượt quá 255 ký tự.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Giá tour phải >= 0.");

            RuleFor(x => x.DurationInDays)
                .NotEmpty().WithMessage("Thời lượng tour là bắt buộc.")
                .Matches(@"^\d+$").WithMessage("Thời lượng tour phải là số nguyên dương.");

            RuleFor(x => x.TourOperatorId)
                .GreaterThan(0).WithMessage("TourOperatorId phải lớn hơn 0.");

            RuleFor(x => x.MaxSlots)
                .GreaterThan(0).WithMessage("Số chỗ tối đa phải lớn hơn 0.");

            RuleFor(x => x.TourType)
                .NotEmpty().WithMessage("Loại tour là bắt buộc.")
                .Must(type => type == "Private" || type == "Share")
                .WithMessage("Loại tour chỉ có thể là 'Private' hoặc 'Share'.");

            RuleFor(x => x.TourStatus)
                .Must(s => string.IsNullOrEmpty(s) || s == "Active" || s == "Inactive")
                .WithMessage("Trạng thái tour chỉ có thể là 'Active' hoặc 'Inactive'.");

            RuleFor(x => x.DepartureDates)
                .NotNull().WithMessage("Danh sách ngày khởi hành là bắt buộc.")
                .Must(list => list.Count > 0).WithMessage("Phải có ít nhất 1 ngày khởi hành.");

            RuleForEach(x => x.DepartureDates)
                .SetValidator(new DepartureDateCreateDtoValidator());

            RuleForEach(x => x.TourExperiences)
                .SetValidator(new TourExperienceCreateDtoValidator());

            RuleForEach(x => x.TourMedia)
                .SetValidator(new TourMediaCreateDtoValidator());

            RuleForEach(x => x.TourItineraries)
                .SetValidator(new TourItineraryCreateDtoValidator());
        }
    }
}
