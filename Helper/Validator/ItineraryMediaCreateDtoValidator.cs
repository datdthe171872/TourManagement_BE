using FluentValidation;
using TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Create;

namespace TourManagement_BE.Helper.Validator
{
    public class ItineraryMediaCreateDtoValidator : AbstractValidator<ItineraryMediaCreateDto>
    {
        public ItineraryMediaCreateDtoValidator()
        {
            RuleFor(x => x.MediaFile)
                .NotNull().WithMessage("File media là bắt buộc.");

            RuleFor(x => x.MediaType)
                .NotEmpty().WithMessage("Loại media là bắt buộc.")
                .Must(type => type == "Image" || type == "Video")
                .WithMessage("Loại media chỉ có thể là 'Image' hoặc 'Video'.");
        }
    }
}
