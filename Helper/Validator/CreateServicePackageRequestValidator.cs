using FluentValidation;
using TourManagement_BE.Data.DTO.Request.ServicePackageRequest;

namespace TourManagement_BE.Helper.Validator
{
    public class CreateServicePackageRequestValidator : AbstractValidator<CreateServicePackageRequest>
    {
        public CreateServicePackageRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Tên gói dịch vụ là bắt buộc.")
                .MaximumLength(255)
                .WithMessage("Tên không được vượt quá 255 ký tự.");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Mô tả không được vượt quá 1000 ký tự.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Giá phải >= 0.");

            RuleFor(x => x.DiscountPercentage)
                .InclusiveBetween(0, 100)
                .WithMessage("Phần trăm giảm giá phải từ 0 đến 100.")
                .When(x => x.DiscountPercentage.HasValue);
        }
    }
}
