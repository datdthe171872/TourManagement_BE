using AutoMapper;
using TourManagement_BE.Data.DTO.Response.TourBookingResponse;
using TourManagement_BE.Data.Models;
using TourManagement_BE.TourManagement_BE.Data.DTO.Response.TourBookingResponse;

namespace TourManagement_BE.Mapping.TourBookingDetail
{
    public class TourBookingDetailMapping : Profile
    {
        public TourBookingDetailMapping()
        {
            CreateMap<Booking, TourBookingDetailResponse>();

            CreateMap<Payment, PaymentDto>();

            CreateMap<TourAcceptanceReport, TourAcceptanceReportDto>();

            CreateMap<ExtraCharge, ExtraChargeDto>();
        }
    }
}
