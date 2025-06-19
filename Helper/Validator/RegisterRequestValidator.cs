using FluentValidation;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Helper.Constant;

namespace TourManagement_BE.Helper.Validator
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required")
                .MaximumLength(255).WithMessage("Username cannot exceed 255 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?\d{10,15}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Invalid phone number format");

            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role is required")
                .Must(role => Roles.AllRoles.Contains(role))
                .WithMessage("Invalid role. Must be one of: " + string.Join(", ", Roles.AllRoles));
        }
    }
}
