using FluentValidation;
using TourManagement_BE.Data.DTO.Request.DepartureDatesRequest;

namespace TourManagement_BE.Helper.Validator;

public class UpdateDepartureDateRequestValidator : AbstractValidator<UpdateDepartureDateRequest>
{
    public UpdateDepartureDateRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id phải lớn hơn 0");

        RuleFor(x => x.DepartureDate1)
            .NotEmpty()
            .WithMessage("Ngày khởi hành không được để trống")
            .Must(date => date.Date > DateTime.Now.Date)
            .WithMessage("Ngày khởi hành phải là ngày trong tương lai");
    }
}