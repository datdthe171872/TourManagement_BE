using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Response.PaymentResponse;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasedServicePackagesController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;

        public PurchasedServicePackagesController(MyDBContext context, IMapper mapper)
        {
            this.context = context;
            _mapper = mapper;
        }
    }
}
