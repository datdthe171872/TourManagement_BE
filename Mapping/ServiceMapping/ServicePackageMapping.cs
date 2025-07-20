using AutoMapper;
using TourManagement_BE.Data.DTO.Request.ServicePackageRequest;
using TourManagement_BE.Data.DTO.Response.TourResponse;
using TourManagement_BE.Models;

namespace TourManagement_BE.Mapping.ServiceMapping
{
    public class ServicePackageMapping : Profile
    {
        public ServicePackageMapping()
        {
            CreateMap<ServicePackage, CreateServicePackageRequest>();
        }
    }
}
