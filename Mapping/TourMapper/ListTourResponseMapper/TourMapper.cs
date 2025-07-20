using TourManagement_BE.Data.DTO.Response.TourResponse;
using TourManagement_BE.Models;
using AutoMapper;
using TourManagement_BE.Data.DTO.Response.TourResponse.TourDetailDTO;

namespace TourManagement_BE.Mapping.TourMapper.ListTourResponseMapper
{
    public class TourMapper : Profile
    {
        public TourMapper()
        {
            CreateMap<Tour, ListTourResponse>()
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.TourOperator.CompanyName))
            .ForMember(dest => dest.CompanyDescription, opt => opt.MapFrom(src => src.TourOperator.Description))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                src.TourRatings != null && src.TourRatings.Any(r => r.Rating.HasValue)
                    ? Math.Round(src.TourRatings.Where(r => r.Rating.HasValue).Average(r => r.Rating.Value), 1)
                    : (double?)null
            ));


            CreateMap<Tour, TourDetailResponse>()
            .ForMember(dest => dest.TourOperatorName, opt => opt.MapFrom(src => src.TourOperator.User.UserName))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.TourOperator.CompanyName))
            .ForMember(dest => dest.CompanyDescription, opt => opt.MapFrom(src => src.TourOperator.Description))
            .ForMember(dest => dest.CompanyLogo, opt => opt.MapFrom(src => src.TourOperator.CompanyLogo))
            .ForMember(dest => dest.CompanyHotline, opt => opt.MapFrom(src => src.TourOperator.Hotline))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                src.TourRatings != null && src.TourRatings.Any(r => r.Rating.HasValue)
                    ? Math.Round(src.TourRatings.Where(r => r.Rating.HasValue).Average(r => r.Rating.Value), 1)
                    : (double?)null
            ));

            CreateMap<TourExperience, TourExperienceDto>();
            CreateMap<DepartureDate, DepartureDateDto>();
            CreateMap<TourItinerary, TourItineraryDto>();
            CreateMap<TourMedia, TourMediaDto>();
            CreateMap<TourRating, TourRatingDto>()
            .ForMember(dest => dest.TourRating_Username, opt => opt.MapFrom(src => src.User.UserName));
            CreateMap<ItineraryMedia, ItineraryMediaDto>();




        }
    }
}
