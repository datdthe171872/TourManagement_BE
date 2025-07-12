using FluentValidation;
using TourManagement_BE.Data.DTO.Request.TourItineraryRequest;

namespace TourManagement_BE.Helper.Validator
{
    public class ItineraryMediaCreateValidator : AbstractValidator<ItineraryMediaCreate>
    {
        public ItineraryMediaCreateValidator()
        {
            RuleFor(x => x.ItineraryId)
                .GreaterThan(0)
                .WithMessage("ItineraryId phải lớn hơn 0.");

            RuleFor(x => x.MediaFile)
                .NotNull()
                .WithMessage("MediaFile là bắt buộc.")
                .Must(file => file.Length > 0)
                .WithMessage("MediaFile không được rỗng.");

            RuleFor(x => x.MediaType)
                .NotEmpty()
                .WithMessage("MediaType là bắt buộc.")
                .Must(type => type == "Image" || type == "Video")
                .WithMessage("MediaType phải là 'Image' hoặc 'Video'.");

            RuleFor(x => x.Caption)
                .MaximumLength(500)
                .WithMessage("Caption không được vượt quá 500 ký tự.")
                .When(x => !string.IsNullOrWhiteSpace(x.Caption));
        }
    }

}
