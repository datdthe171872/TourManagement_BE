using FluentValidation;
using TourManagement_BE.Data.DTO.Request.TourRequest;

namespace TourManagement_BE.Helper.Validator
{
    public class TourUpdateRequestValidator : AbstractValidator<TourUpdateRequest>
    {
        public TourUpdateRequestValidator()
        {
            RuleFor(x => x.TourId)
                .GreaterThan(0).WithMessage("TourId không hợp lệ.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tiêu đề tour là bắt buộc.")
                .MaximumLength(255).WithMessage("Tiêu đề tour không được vượt quá 255 ký tự.");

            RuleFor(x => x.PriceOfAdults)
                .GreaterThanOrEqualTo(0).WithMessage("Giá người lớn phải >= 0.");

            RuleFor(x => x.PriceOfChildren)
                .GreaterThanOrEqualTo(0).WithMessage("Giá trẻ em phải >= 0.");

            RuleFor(x => x.PriceOfInfants)
                .GreaterThanOrEqualTo(0).WithMessage("Giá trẻ sơ sinh phải >= 0.");

            RuleFor(x => x.DurationInDays)
                .NotEmpty().WithMessage("Thời lượng tour là bắt buộc.")
                .Matches(@"^\d+$").WithMessage("Thời lượng tour phải là số nguyên dương.");

            RuleFor(x => x.MaxSlots)
                .GreaterThan(0).WithMessage("Số chỗ tối đa phải lớn hơn 0.");

            RuleFor(x => x.MinSlots)
                .GreaterThan(0).WithMessage("Số chỗ tối thiểu phải lớn hơn 0.");

            RuleFor(x => x.TourStatus)
                .NotEmpty().WithMessage("Trạng thái tour là bắt buộc.");

            RuleFor(x => x.DepartureDates)
            .NotNull().WithMessage("Danh sách ngày khởi hành là bắt buộc.")
            .Must(list => list.Count > 0).WithMessage("Phải có ít nhất 1 ngày khởi hành.")
            .Must(list => list.All(date => date.Date >= DateTime.UtcNow.Date.AddMonths(1)))
            .WithMessage("Mỗi ngày khởi hành phải lớn hơn ngày hiện tại ít nhất 1 tháng.");


            RuleFor(x => x.TourItineraries)
                .Must((req, list) => CheckItineraryCount(req.DurationInDays, list))
                .WithMessage(req => $"Số ngày lịch trình không đúng với DurationInDays = {req.DurationInDays}.");

            // Validate từng phần tử
            /*RuleForEach(x => x.DepartureDates)
                .SetValidator(new DepartureDateDto());

            RuleForEach(x => x.TourExperiences)
                .SetValidator(new TourExperienceCreateDtoValidator());

            RuleForEach(x => x.TourMedia)
                .SetValidator(new TourMediaCreateDtoValidator());

            RuleForEach(x => x.TourItineraries)
                .SetValidator(new TourItineraryCreateDtoValidator());*/
        }

        private bool CheckItineraryCount<T>(string durationStr, IList<T> itineraries)
        {
            if (string.IsNullOrWhiteSpace(durationStr) || itineraries == null)
                return true;

            if (!int.TryParse(durationStr, out var days))
                return false;

            // Đảm bảo chỉ tính những cái IsActive = true (nếu bạn muốn giới hạn như thế)
            return itineraries.Count == days;
        }
    }

}
