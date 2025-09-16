using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class GuideNoteController : ControllerBase
    {
        private readonly IGuideNoteService _guideNoteService;

        public GuideNoteController(IGuideNoteService guideNoteService)
        {
            _guideNoteService = guideNoteService;
        }

        // Lấy danh sách note của guide hiện tại
        [HttpGet("notes")]
        public async Task<ActionResult<List<GuideNoteResponse>>> GetMyNotes()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var notes = await _guideNoteService.GetNotesByGuideUserIdAsync(userId);
            return Ok(notes);
        }

        // Thêm note mới
        [HttpPost("notes")]
        public async Task<ActionResult> CreateNote([FromBody] CreateGuideNoteRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _guideNoteService.CreateNoteAsync(userId, request);
            return Ok("Note created successfully");
        }

        // Sửa note
        [HttpPut("notes/{id}")]
        public async Task<ActionResult> UpdateNote(int id, [FromBody] UpdateGuideNoteRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _guideNoteService.UpdateNoteAsync(userId, id, request);
            return Ok("Note updated successfully");
        }

        // Xoá note
        [HttpDelete("notes/{id}")]
        public async Task<ActionResult> DeleteNote(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _guideNoteService.DeleteNoteAsync(userId, id);
            return Ok("Note deleted successfully");
        }
    }
} 