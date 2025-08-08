using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.TourContract;
using TourManagement_BE.Data.DTO.Response.ContractTourBooking;

namespace TourManagement_BE.Service.ContractManage
{
    public class ContractService : IContractService
    {
        private readonly MyDBContext _context;
        private readonly Cloudinary _cloudinary;
        private readonly IMapper _mapper;

        public ContractService(MyDBContext context, Cloudinary cloudinary, IMapper mapper)
        {
            _context = context;
            _cloudinary = cloudinary;
            _mapper = mapper;
        }

        public async Task<ContractTourBookingResponse> GetContractByBookingIdAsync(int bookingId)
        {
            var contract = await _context.Bookings
                .Where(c => c.BookingId == bookingId)
                .Select(c => new ContractTourBookingResponse
                {
                    BookingId = c.BookingId,
                    TourId = c.TourId,
                    Contract = c.Contract
                })
                .FirstOrDefaultAsync();

            return contract;
        }

        public async Task<OperationResult> CreateContractAsync(CreateContractRequest request)
        {
            var result = new OperationResult();

            try
            {
                var booking = await _context.Bookings.FindAsync(request.BookingId);
                if (booking == null)
                {
                    result.Success = false;
                    result.Message = "Booking not found.";
                    return result;
                }

                if (request.Contract == null || request.Contract.Length == 0)
                {
                    result.Success = false;
                    result.Message = "Contract file is required.";
                    return result;
                }

                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(request.Contract.FileName, request.Contract.OpenReadStream()),
                    Folder = "ProjectSEP490/TourBooking/Contract",
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                booking.Contract = uploadResult.SecureUrl.ToString();
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Contract created successfully.";
                result.Contract = _mapper.Map<ContractTourBookingResponse>(booking);

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Create Contract Fail: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult> UpdateContractAsync(UpdateTourContractRequest request)
        {
            var result = new OperationResult();

            try
            {
                var booking = await _context.Bookings.FindAsync(request.BookingId);
                if (booking == null)
                {
                    result.Success = false;
                    result.Message = "Booking not found.";
                    return result;
                }

                if (request.Contract == null || request.Contract.Length == 0)
                {
                    result.Success = false;
                    result.Message = "Contract file is required.";
                    return result;
                }

                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(request.Contract.FileName, request.Contract.OpenReadStream()),
                    Folder = "ProjectSEP490/TourBooking/Contract",
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                booking.Contract = uploadResult.SecureUrl.ToString();
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Contract updated successfully.";
                result.Contract = _mapper.Map<ContractTourBookingResponse>(booking);

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Update Contract Fail: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult> DeleteContractAsync(int bookingId)
        {
            var result = new OperationResult();

            try
            {
                var booking = await _context.Bookings.FindAsync(bookingId);
                if (booking == null)
                {
                    result.Success = false;
                    result.Message = "Booking not found.";
                    return result;
                }

                if (string.IsNullOrEmpty(booking.Contract))
                {
                    result.Success = false;
                    result.Message = "No contract to delete.";
                    return result;
                }

                booking.Contract = null;
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Contract deleted successfully.";
                result.Contract = _mapper.Map<ContractTourBookingResponse>(booking);

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Delete Contract Fail: {ex.Message}";
                return result;
            }
        }
    }
}
