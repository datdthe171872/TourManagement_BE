using FluentValidation;
using TourManagement_BE.Data.DTO.Request.ProfileRequest;

namespace TourManagement_BE.Helper.Validator
{
    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileRequestValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("UserId phải lớn hơn 0.");

            RuleFor(x => x.UserName)
                .MaximumLength(255)
                .WithMessage("Tên người dùng không được vượt quá 255 ký tự.")
                .When(x => !string.IsNullOrWhiteSpace(x.UserName));

            RuleFor(x => x.Email)
                .MaximumLength(255)
                .WithMessage("Email không được vượt quá 255 ký tự.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Email không hợp lệ.")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Address)
                .MaximumLength(500)
                .WithMessage("Địa chỉ không được vượt quá 500 ký tự.")
                .When(x => !string.IsNullOrWhiteSpace(x.Address));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^0[0-9]{8,10}$")
                .WithMessage("Số điện thoại phải bắt đầu bằng số 0 và có độ dài từ 9 đến 11 chữ số.")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));


            RuleFor(x => x.AvatarFile)
                .Must(f =>
                {
                    if (f == null) return true;
                    var allowed = new[] { ".jpg", ".jpeg", ".png" };
                    var ext = Path.GetExtension(f.FileName).ToLowerInvariant();
                    return allowed.Contains(ext);
                })
                .WithMessage("Ảnh đại diện phải là file JPG, JPEG hoặc PNG.");

            RuleFor(x => x.AvatarFile)
                .Must(f => f == null || f.Length <= 5 * 1024 * 1024)
                .WithMessage("Ảnh đại diện không được vượt quá 5MB.");
        }
    }

}
