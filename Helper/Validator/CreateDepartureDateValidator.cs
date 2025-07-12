using FluentValidation;
using TourManagement_BE.Data.DTO.Request.DepartureDatesRequest;

namespace TourManagement_BE.Helper.Validator
{
    public class CreateDepartureDateValidator : AbstractValidator<CreateDepartureDate>
    {
        public CreateDepartureDateValidator()
        {
            RuleFor(x => x.TourId)
                .GreaterThan(0)
                .WithMessage("TourId phải lớn hơn 0.");

            RuleFor(x => x.DepartureDate1)
                .Must(date => date.Date >= DateTime.UtcNow.Date)
                .WithMessage("Ngày khởi hành không được nhỏ hơn ngày hiện tại.");
        }
    }

}
