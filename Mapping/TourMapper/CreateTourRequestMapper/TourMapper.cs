using TourManagement_BE.Data.Models;
using AutoMapper;
using TourManagement_BE.Data.DTO.Request.TourRequest;
using TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Update;

namespace TourManagement_BE.Mapping.TourMapper.CreateTourRequestMapper
{
    public class TourMapper : Profile
    {
        public TourMapper()
        {
            CreateMap<TourCreateRequest, Tour>();

            CreateMap<TourExperience, TourExperienceDto>();
            CreateMap<DepartureDate, DepartureDateDto>();
            CreateMap<TourItinerary, TourItineraryDto>();
            CreateMap<TourMedia, TourMediaDto>();
            CreateMap<ItineraryMedia, ItineraryMediaDto>();

        }
    }
}
