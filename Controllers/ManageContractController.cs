using AutoMapper;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data;
using TourManagement_BE.Data.DTO.Request.ServicePackageRequest;
using TourManagement_BE.Data.DTO.Response.ContractTourBooking;
using TourManagement_BE.Data.DTO.Request.TourContract;
using TourManagement_BE.Models;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageContractController : Controller
    {
        private readonly MyDBContext context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        public ManageContractController(MyDBContext context, IMapper mapper, Cloudinary cloudinary)
        {
            this.context = context;
            _mapper = mapper;
            this._cloudinary = cloudinary;
        }

        [HttpGet("ViewContractDetail/{bookingid}")]

        public async Task<IActionResult> ViewContractDetail(int bookingid)
        {
            var contract = context.Bookings.Where(c => c.BookingId == bookingid)
                .Select(c => new ContractTourBookingResponse
                {
                    BookingId = c.BookingId,
                    TourId = c.TourId,
                    Contract = c.Contract
                }).
                FirstOrDefault();
            if (contract == null)
            {
                return NotFound("Contract has not been added to this Tour Booking.");
            }

            return Ok(contract);
        }

        [HttpPost("CreateContractForTourBooking")]
        public async Task<IActionResult> CreateContract([FromForm] CreateContractRequest request)
        {
            try
            {
                var booking = await context.Bookings.FindAsync(request.BookingId);
                if (booking == null)
                    return NotFound("Booking not found.");

                if (request.Contract == null || request.Contract.Length == 0)
                    return BadRequest("Contract file is required.");

                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(request.Contract.FileName, request.Contract.OpenReadStream()),
                    Folder = "ProjectSEP490/TourBooking/Contract",
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                var uploadedUrl = uploadResult.SecureUrl.ToString();

                booking.Contract = uploadedUrl;

                await context.SaveChangesAsync();
                return Ok(new { message = "Create Contract successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Create Contract Fail.", error = ex.Message });
            }
        }

        [HttpPut("UpdateContractForTourBooking")]
        public async Task<IActionResult> UpdateContract([FromForm] UpdateTourContractRequest request)
        {
            try
            {
                var booking = await context.Bookings.FindAsync(request.BookingId);
                if (booking == null)
                    return NotFound("Booking not found.");

                if (request.Contract == null || request.Contract.Length == 0)
                    return BadRequest("Contract file is required.");

                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(request.Contract.FileName, request.Contract.OpenReadStream()),
                    Folder = "ProjectSEP490/TourBooking/Contract",
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                var uploadedUrl = uploadResult.SecureUrl.ToString();

                booking.Contract = uploadedUrl;

                await context.SaveChangesAsync();
                return Ok(new { message = "Contract updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Update Contract Fail.", error = ex.Message });
            }
        }

        [HttpDelete("DeleteContractForTourBooking/{bookingId}")]
        public async Task<IActionResult> DeleteContract(int bookingId)
        {
            try
            {
                var booking = await context.Bookings.FindAsync(bookingId);
                if (booking == null)
                    return NotFound("Booking not found.");

                if (string.IsNullOrEmpty(booking.Contract))
                    return BadRequest("No contract to delete.");

                booking.Contract = null;

                await context.SaveChangesAsync();
                return Ok(new { message = "Contract deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Delete Contract Fail.", error = ex.Message });
            }
        }


    }
}
