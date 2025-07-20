using TourManagement_BE.Models;
using AutoMapper;
using TourManagement_BE.Data.DTO.Request.TourRequest;
using TourManagement_BE.Data.DTO.Request.TourRequest.TourRequestDTO.Update;

namespace TourManagement_BE.Mapping.TourMapper.UpdateTourRequestMapper
{
    public class TourMapper : Profile
    {
        public TourMapper()
        {
            CreateMap<TourUpdateRequest, Tour>();

            CreateMap<TourExperience, TourExperienceDto>();
            CreateMap<DepartureDate, DepartureDateDto>();
            CreateMap<TourItinerary, TourItineraryDto>();
            CreateMap<TourMedia, TourMediaDto>();
            CreateMap<ItineraryMedia, ItineraryMediaDto>();

        }
    }
}
