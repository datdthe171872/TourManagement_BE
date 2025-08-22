using FluentValidation;
using TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Create;

namespace TourManagement_BE.Helper.Validator
{
    public class DepartureDateCreateDtoValidator : AbstractValidator<DepartureDateCreateDto>
    {
        public DepartureDateCreateDtoValidator()
        {
            RuleFor(x => x.DepartureDate1)
            .Must(date => date.Date >= DateTime.UtcNow.Date.AddMonths(1))
            .WithMessage("Ngày khởi hành phải lớn hơn ngày hiện tại ít nhất 1 tháng.");
        }
    }
}
