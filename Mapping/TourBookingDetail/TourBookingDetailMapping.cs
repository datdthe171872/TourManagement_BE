using AutoMapper;
using TourManagement_BE.Data.DTO.Response.TourBookingDetailResponse;
using TourManagement_BE.Data.DTO.Response.TourBookingResponse;
using TourManagement_BE.Data.Models;
using TourManagement_BE.TourManagement_BE.Data.DTO.Response.TourBookingResponse;

namespace TourManagement_BE.Mapping.TourBookingDetail
{
    public class TourBookingDetailMapping : Profile
    {
        public TourBookingDetailMapping()
        {
            CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.PaymentTypeName, opt => opt.MapFrom(src => src.PaymentType.PaymentTypeName));

            CreateMap<TourAcceptanceReport, TourAcceptanceReportDto>();



            CreateMap<Booking, TourBookingDetailResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.DepartureDate, opt => opt.MapFrom(src => src.DepartureDate.DepartureDate1))
                .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments.Where(p => p.IsActive)))
                .ForMember(dest => dest.AcceptanceReport, opt => opt.MapFrom(src =>
                    src.TourAcceptanceReports.FirstOrDefault(r => r.IsActive)));
        }
    }
}
