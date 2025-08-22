using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.ServicePackageRequest;
using TourManagement_BE.Data.DTO.Request.TourContract;
using TourManagement_BE.Data.DTO.Response.ContractTourBooking;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Helper.Constant;
using TourManagement_BE.Service.ContractManage;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ManageContractController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly IContractService _contractService;
        public ManageContractController(MyDBContext context, IMapper mapper, Cloudinary cloudinary, IContractService contractService)
        {
            this.context = context;
            _mapper = mapper;
            this._cloudinary = cloudinary;
            _contractService = contractService;
        }

        [HttpGet("ViewContractDetail/{bookingid}")]

        public async Task<IActionResult> ViewContractDetail(int bookingid)
        {
            var contract = await _contractService.GetContractByBookingIdAsync(bookingid);
            if (contract == null)
            {
                return NotFound("Contract has not been added to this Tour Booking.");
            }
            return Ok(contract);
        }

        [HttpPost("CreateContractForTourBooking")]
        [Authorize(Roles = Roles.TourOperator)]
        public async Task<IActionResult> CreateContract([FromForm] CreateContractRequest request)
        {
            var result = await _contractService.CreateContractAsync(request);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(new { message = result.Message, contract = result.Contract });
        }

        [HttpPut("UpdateContractForTourBooking")]
        [Authorize(Roles = Roles.TourOperator)]
        public async Task<IActionResult> UpdateContract([FromForm] UpdateTourContractRequest request)
        {
            var result = await _contractService.UpdateContractAsync(request);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(new { message = result.Message, contract = result.Contract });
        }

        [HttpDelete("DeleteContractForTourBooking/{bookingId}")]
        [Authorize(Roles = Roles.TourOperator)]
        public async Task<IActionResult> DeleteContract(int bookingId)
        {
            var result = await _contractService.DeleteContractAsync(bookingId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(new { message = result.Message, contract = result.Contract });
        }

    }
}
