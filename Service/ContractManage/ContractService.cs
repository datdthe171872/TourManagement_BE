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
                    result.Message = "Không tìm thấy đặt chỗ.";
                    return result;
                }

                if (request.Contract == null || request.Contract.Length == 0)
                {
                    result.Success = false;
                    result.Message = "Cần phải có hồ sơ hợp đồng.";
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
                result.Message = "Hợp đồng đã được tạo thành công.";
                result.Contract = _mapper.Map<ContractTourBookingResponse>(booking);

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Tạo bị lỗi: {ex.Message}";
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
                    return new OperationResult { Success = false, Message = "Không tìm thấy đặt chỗ." };
                }

                if (request.Contract == null || request.Contract.Length == 0)
                {
                    return new OperationResult { Success = false, Message = "Cần phải có hồ sơ hợp đồng." };
                }

                const long maxFileSize = 10 * 1024 * 1024;
                if (request.Contract.Length > maxFileSize)
                {
                    return new OperationResult { Success = false, Message = "Kích thước tệp hợp đồng không được vượt quá 10MB." };
                }

                if (_cloudinary == null)
                {
                    return new OperationResult { Success = false, Message = "Cloudinary service not initialized (_cloudinary is null)." };
                }

                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(request.Contract.FileName, request.Contract.OpenReadStream()),
                    Folder = "ProjectSEP490/TourBooking/Contract",
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult == null || uploadResult.SecureUrl == null)
                {
                    return new OperationResult { Success = false, Message = "Tải lên Cloudinary không thành công (uploadResult là null)." };
                }

                booking.Contract = uploadResult.SecureUrl.ToString();
                await _context.SaveChangesAsync();

                if (_mapper == null)
                {
                    return new OperationResult { Success = false, Message = "Mapper chưa được khởi tạo (_mapper là null)." };
                }

                return new OperationResult
                {
                    Success = true,
                    Message = "Hợp đồng đã được cập nhật thành công.",
                    Contract = _mapper.Map<ContractTourBookingResponse>(booking)
                };
            }
            catch (Exception ex)
            {
                return new OperationResult { Success = false, Message = $"Cập nhật hợp đồng thất bại: {ex.Message}" };
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
                    result.Message = "Không tìm thấy đặt chỗ.";
                    return result;
                }

                if (string.IsNullOrEmpty(booking.Contract))
                {
                    result.Success = false;
                    result.Message = "Không có hợp đồng nào để xóa.";
                    return result;
                }

                booking.Contract = null;
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Hợp đồng đã được xóa thành công.";
                result.Contract = _mapper.Map<ContractTourBookingResponse>(booking);

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Xóa hợp đồng không thành công: {ex.Message}";
                return result;
            }
        }
    }
}
