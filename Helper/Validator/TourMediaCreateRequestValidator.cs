using FluentValidation;
using TourManagement_BE.Data.DTO.Request.TourMediaRequest;

namespace TourManagement_BE.Helper.Validator
{
    public class TourMediaCreateRequestValidator : AbstractValidator<TourMediaCreateRequest>
    {
        public TourMediaCreateRequestValidator()
        {
            RuleFor(x => x.TourId)
                .GreaterThan(0)
                .WithMessage("TourId phải lớn hơn 0.");

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
        }
    }

}
