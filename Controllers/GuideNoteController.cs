using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Service;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bỏ role restriction ở class level, để kiểm soát ở từng method
    public class GuideNoteController : ControllerBase
    {
        private readonly IGuideNoteService _guideNoteService;

        public GuideNoteController(IGuideNoteService guideNoteService)
        {
            _guideNoteService = guideNoteService;
        }

        // Lấy danh sách note của guide hiện tại
        [HttpGet("notes")]
        [Authorize(Roles = "Tour Guide")]
        public async Task<ActionResult<List<GuideNoteResponse>>> GetMyNotes()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var notes = await _guideNoteService.GetNotesByGuideUserIdAsync(userId);
                return Ok(notes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Thêm note mới cho TourGuide
        [HttpPost("notes")]
        [Authorize(Roles = "Tour Guide")]
        public async Task<ActionResult> CreateNote([FromBody] CreateGuideNoteByTourGuideRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _guideNoteService.CreateNoteByTourGuideAsync(userId, request);
                return Ok(new { message = "Note created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Sửa note
        [HttpPut("notes/{id}")]
        [Authorize(Roles = "Tour Guide")]
        public async Task<ActionResult> UpdateNote(int id, [FromBody] UpdateGuideNoteRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _guideNoteService.UpdateNoteAsync(userId, id, request);
                return Ok(new { message = "Note updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Xoá note
        [HttpDelete("notes/{id}")]
        [Authorize(Roles = "Tour Guide")]
        public async Task<ActionResult> DeleteNote(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _guideNoteService.DeleteNoteAsync(userId, id);
                return Ok(new { message = "Note deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Lấy danh sách booking của TourGuide
        [HttpGet("my-bookings")]
        [Authorize(Roles = "Tour Guide")]
        public async Task<ActionResult<List<TourGuideBookingResponse>>> GetMyBookings()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var bookings = await _guideNoteService.GetMyBookingsAsync(userId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Upload ảnh cho GuideNote
        [HttpPost("upload-attachment")]
        [Authorize(Roles = "Tour Guide")]
        public async Task<ActionResult> UploadAttachment(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded" });
                }

                // Kiểm tra file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = "Invalid file type. Allowed: jpg, jpeg, png, gif, pdf" });
                }

                // Kiểm tra file size (max 10MB)
                if (file.Length > 10 * 1024 * 1024)
                {
                    return BadRequest(new { message = "File size too large. Maximum 10MB allowed" });
                }

                // Tạo unique filename
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadPath = Path.Combine("wwwroot", "uploads", "guidenotes");
                
                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);
                
                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Trả về URL
                var fileUrl = $"/uploads/guidenotes/{fileName}";
                
                return Ok(new { 
                    message = "File uploaded successfully",
                    attachmentUrl = fileUrl,
                    fileName = fileName
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // TourOperator lấy tất cả note của TourGuide
        [HttpGet("tour-operator/notes")]
        [Authorize(Roles = "Tour Operator")]
        public async Task<ActionResult<List<GuideNoteResponse>>> GetNotesByTourOperator()
        {
            try
            {
                var tourOperatorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var notes = await _guideNoteService.GetNotesByTourOperatorAsync(tourOperatorId);
                return Ok(notes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // TourOperator update extra cost của GuideNote
        [HttpPut("notes/{noteId}/extra-cost")]
        [Authorize(Roles = "Tour Operator")]
        public async Task<ActionResult> UpdateNoteExtraCost(int noteId, [FromBody] UpdateGuideNoteExtraCostRequest request)
        {
            try
            {
                var tourOperatorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _guideNoteService.UpdateNoteExtraCostAsync(tourOperatorId, noteId, request);
                return Ok(new { message = "Extra cost updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 