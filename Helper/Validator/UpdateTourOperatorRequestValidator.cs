using FluentValidation;
using TourManagement_BE.Data.DTO.Request;

namespace TourManagement_BE.Helper.Validator;

public class UpdateTourOperatorRequestValidator : AbstractValidator<UpdateTourOperatorRequest>
{
    public UpdateTourOperatorRequestValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Tên công ty là bắt buộc")
            .MaximumLength(255)
            .WithMessage("Tên công ty không được vượt quá 255 ký tự");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Mô tả không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.CompanyLogo)
            .MaximumLength(500)
            .WithMessage("Logo công ty không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.CompanyLogo));

        RuleFor(x => x.LicenseNumber)
            .MaximumLength(100)
            .WithMessage("Số giấy phép không được vượt quá 100 ký tự")
            .When(x => !string.IsNullOrEmpty(x.LicenseNumber));

        RuleFor(x => x.TaxCode)
            .MaximumLength(50)
            .WithMessage("Mã số thuế không được vượt quá 50 ký tự")
            .When(x => !string.IsNullOrEmpty(x.TaxCode));

        RuleFor(x => x.EstablishedYear)
            .InclusiveBetween(1900, 2100)
            .WithMessage("Năm thành lập phải từ 1900 đến 2100")
            .When(x => x.EstablishedYear.HasValue);

        RuleFor(x => x.Hotline)
            .MaximumLength(20)
            .WithMessage("Hotline không được vượt quá 20 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Hotline));

        RuleFor(x => x.Website)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Website phải là URL hợp lệ")
            .When(x => !string.IsNullOrEmpty(x.Website));

        RuleFor(x => x.Facebook)
            .MaximumLength(255)
            .WithMessage("Facebook không được vượt quá 255 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Facebook));

        RuleFor(x => x.Instagram)
            .MaximumLength(255)
            .WithMessage("Instagram không được vượt quá 255 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Instagram));

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .WithMessage("Địa chỉ không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Address));

        RuleFor(x => x.WorkingHours)
            .MaximumLength(200)
            .WithMessage("Giờ làm việc không được vượt quá 200 ký tự")
            .When(x => !string.IsNullOrEmpty(x.WorkingHours));
    }
} 