using FluentValidation;
using TourManagement_BE.Data.DTO.Request.DepartureDatesRequest;

namespace TourManagement_BE.Helper.Validator;

public class CreateDepartureDateRequestValidator : AbstractValidator<CreateDepartureDateRequest>
{
    public CreateDepartureDateRequestValidator()
    {
        RuleFor(x => x.TourId)
            .GreaterThan(0)
            .WithMessage("TourId phải lớn hơn 0");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Ngày bắt đầu không được để trống")
            .Must(date => date.Date > DateTime.Now.Date)
            .WithMessage("Ngày bắt đầu phải là ngày trong tương lai");
    }
} 