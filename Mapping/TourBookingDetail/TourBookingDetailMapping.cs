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

            //CreateMap<ExtraCharge, ExtraChargeDto>();

            //CreateMap<BookingExtraCharge, BookingExtraChargesDTO>()
            //    .ForMember(dest => dest.ExtraCharge, opt => opt.Ignore())
            //    .AfterMap((src, dest) =>
            //    {
            //        dest.ExtraCharge = new List<ExtraChargeDto> { new ExtraChargeDto
            //        {
            //            ExtraChargeId = src.ExtraCharge.ExtraChargeId,
            //            Name = src.ExtraCharge.Name,
            //            Description = src.ExtraCharge.Description,
            //            Amount = src.ExtraCharge.Amount,
            //            IsActive = src.ExtraCharge.IsActive
            //        }};
            //    });

            CreateMap<Booking, TourBookingDetailResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.DepartureDate, opt => opt.MapFrom(src => src.DepartureDate.DepartureDate1))
                .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments.Where(p => p.IsActive)))
                .ForMember(dest => dest.AcceptanceReport, opt => opt.MapFrom(src =>
                    src.TourAcceptanceReports.FirstOrDefault(r => r.IsActive)));
                //.ForMember(dest => dest.BookingExtraCharges, opt => opt.MapFrom(src =>
                //    src.BookingExtraCharges.Where(bec => bec.IsActive)));
        }
    }
}
