using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data.DTO.Request.Chat;
using TourManagement_BE.Service;

namespace TourManagement_BE.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ChatbotController : ControllerBase
	{
		private readonly IChatbotService _chatbotService;

		public ChatbotController(IChatbotService chatbotService)
		{
			_chatbotService = chatbotService;
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Chat([FromBody] ChatRequest request)
		{
			if (request == null || string.IsNullOrWhiteSpace(request.Message))
			{
				return BadRequest("Nội dung trống");
			}
			// Lấy userId từ token nếu có (đã đăng nhập), tránh phải truyền trong body
			var userIdClaim = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
			if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var uid))
			{
				request.UserId = uid;
			}
			var result = await _chatbotService.ProcessMessageAsync(request);
			return Ok(result);
		}
	}
}


