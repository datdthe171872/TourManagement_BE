using TourManagement_BE.Data.DTO.Response.TourResponse;
using TourManagement_BE.Data.Models;
using AutoMapper;
using TourManagement_BE.Data.DTO.Response.TourResponse.TourDetailDTO;

namespace TourManagement_BE.Mapping
{
    public class TourMapper : Profile
    {
        public TourMapper()
        {
            CreateMap<Tour, ListTourResponse>()
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.TourOperator.CompanyName))
            .ForMember(dest => dest.CompanyDescription, opt => opt.MapFrom(src => src.TourOperator.Description));


            CreateMap<Tour, TourDetailResponse>()
            .ForMember(dest => dest.TourOperatorName, opt => opt.MapFrom(src => src.TourOperator.User.UserName))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.TourOperator.CompanyName))
            .ForMember(dest => dest.CompanyDescription, opt => opt.MapFrom(src => src.TourOperator.Description))
            .ForMember(dest => dest.CompanyLogo, opt => opt.MapFrom(src => src.TourOperator.CompanyLogo))
            .ForMember(dest => dest.CompanyHotline, opt => opt.MapFrom(src => src.TourOperator.Hotline));

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
