using AutoMapper;
using TourManagement_BE.Data.DTO.Response.PaymentResponse;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Mapping.PaymentMapping
{
    public class ListAllPaymentMapping : Profile
    {
        public ListAllPaymentMapping()
        {
            CreateMap<Payment, ViewAllPaymentHistory>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.PaymentTypeName, opt => opt.MapFrom(src => src.PaymentType.PaymentTypeName))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.User.Role.RoleName));
        }
    }
}
