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

            RuleFor(x => x.PriceOfAdults).GreaterThanOrEqualTo(0).WithMessage("Giá phải >= 0.");
            RuleFor(x => x.PriceOfChildren).GreaterThanOrEqualTo(0).WithMessage("Giá phải >= 0.");
            RuleFor(x => x.PriceOfInfants).GreaterThanOrEqualTo(0).WithMessage("Giá phải >= 0.");

            // Kiểm tra DurationInDays là số nguyên dương
            RuleFor(x => x.DurationInDays)
                .NotEmpty().WithMessage("Thời lượng tour là bắt buộc.")
                .Matches(@"^\d+$").WithMessage("Thời lượng tour phải là số nguyên dương.");

            RuleFor(x => x.TourOperatorId)
                .GreaterThan(0).WithMessage("TourOperatorId phải lớn hơn 0.");

            RuleFor(x => x.MaxSlots)
                .GreaterThan(0).WithMessage("Số chỗ tối đa phải lớn hơn 0.");

            RuleFor(x => x.MinSlots)
                .GreaterThan(0).WithMessage("Số chỗ tối thiểu phải lớn hơn 0.");

            RuleFor(x => x.TourItineraries)
                .Must((req, list) => CheckListCount(req.DurationInDays, list))
                .WithMessage(req => $"TourItineraries phải có đúng {req.DurationInDays} ngày lịch trình.");

            // Validate từng phần tử

            RuleForEach(x => x.TourExperiences)
                .SetValidator(new TourExperienceCreateDtoValidator());

            RuleForEach(x => x.TourMedia)
                .SetValidator(new TourMediaCreateDtoValidator());

            RuleForEach(x => x.TourItineraries)
                .SetValidator(new TourItineraryCreateDtoValidator());
        }

        private bool CheckListCount<T>(string durationStr, IList<T> list)
        {
            if (string.IsNullOrEmpty(durationStr) || list == null) return true;
            if (!int.TryParse(durationStr, out var days)) return true;
            return list.Count == days;
        }
    }
}
