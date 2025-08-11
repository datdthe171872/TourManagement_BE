using AutoMapper;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response.ContractTourBooking;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterRequest, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar));

            CreateMap<Booking, ContractTourBookingResponse>();
        }
    }
}
