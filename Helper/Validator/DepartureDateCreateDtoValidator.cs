using FluentValidation;
using TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Create;

namespace TourManagement_BE.Helper.Validator
{
    public class DepartureDateCreateDtoValidator : AbstractValidator<DepartureDateCreateDto>
    {
        public DepartureDateCreateDtoValidator()
        {
            RuleFor(x => x.DepartureDate1)
                .Must(date => date.Date >= DateTime.UtcNow.Date)
                .WithMessage("Ngày khởi hành không được nhỏ hơn ngày hiện tại.");
        }
    }
}
