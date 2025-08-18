namespace TourManagement_BE.Data.DTO.Request.Chat
{
	public class ChatRequest
	{
		public string Message { get; set; } = string.Empty;
		public int? UserId { get; set; }
	}
}


