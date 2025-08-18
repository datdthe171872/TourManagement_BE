using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TourManagement_BE.Data.DTO.Request.Chat;

namespace TourManagement_BE.Service
{
	public class GeminiClient : IGeminiClient
	{
		private readonly HttpClient _httpClient;
		private readonly string? _apiKey;

		public GeminiClient(IConfiguration configuration)
		{
			_httpClient = new HttpClient();
			_apiKey = configuration["Gemini:ApiKey"]; // thêm vào appsettings nếu dùng
		}

		public async Task<ParsedCriteria?> ExtractCriteriaAsync(string userMessage)
		{
			if (string.IsNullOrWhiteSpace(_apiKey)) return null;

			// Gọi Gemini 1.5 Flash JSON schema (giả lập theo REST phổ biến); nhà cung cấp có thể khác endpoint
			var endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key=" + _apiKey;
			var systemPrompt = "Bạn là bộ trích xuất tiêu chí tour. Trả về JSON với các trường: destination, dateFrom(yyyy-MM-dd), dateTo(yyyy-MM-dd), durationDays, groupSize, budgetMin, budgetMax, style, startPoint, transportation. Bỏ qua nếu không có.";
			var req = new
			{
				contents = new[]
				{
					new { role = "user", parts = new object[]{ new { text = systemPrompt + "\n\nUser: " + userMessage } } }
				},
				generationConfig = new { responseMimeType = "application/json" }
			};

			using var httpReq = new HttpRequestMessage(HttpMethod.Post, endpoint)
			{
				Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
			};
			var httpRes = await _httpClient.SendAsync(httpReq);
			if (!httpRes.IsSuccessStatusCode) return null;
			var doc = await JsonDocument.ParseAsync(await httpRes.Content.ReadAsStreamAsync());
			// Parse đơn giản; tuỳ response structure của Gemini
			try
			{
				var jsonText = doc.RootElement.GetProperty("candidates")[0]
					.GetProperty("content").GetProperty("parts")[0]
					.GetProperty("text").GetString();
				if (string.IsNullOrWhiteSpace(jsonText)) return null;
				var parsed = JsonSerializer.Deserialize<ParsedCriteria>(jsonText!, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});
				return parsed;
			}
			catch
			{
				return null;
			}
		}

		public async Task<ParsedIntent?> ExtractIntentAsync(string userMessage)
		{
			if (string.IsNullOrWhiteSpace(_apiKey)) return null;
			var endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key=" + _apiKey;
			var systemPrompt = "Phân loại intent cho chatbot tour. Trả JSON fields: intent in [suggest, tour_detail, compare_tours, plan_booking, booking_status, tour_status, help], tourId, tour1Id, tour2Id, bookingId, plan{tourId, departureDateId, numberOfAdults, numberOfChildren, numberOfInfants, confirm}, tourTitle, tourOperatorId, tourOperatorName. Chỉ trả JSON.";
			var req = new
			{
				contents = new[]
				{
					new { role = "user", parts = new object[]{ new { text = systemPrompt + "\n\nUser: " + userMessage } } }
				},
				generationConfig = new { responseMimeType = "application/json" }
			};
			using var httpReq = new HttpRequestMessage(HttpMethod.Post, endpoint)
			{
				Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json")
			};
			var httpRes = await _httpClient.SendAsync(httpReq);
			if (!httpRes.IsSuccessStatusCode) return null;
			var doc = await JsonDocument.ParseAsync(await httpRes.Content.ReadAsStreamAsync());
			try
			{
				var jsonText = doc.RootElement.GetProperty("candidates")[0]
					.GetProperty("content").GetProperty("parts")[0]
					.GetProperty("text").GetString();
				if (string.IsNullOrWhiteSpace(jsonText)) return null;
				var parsed = JsonSerializer.Deserialize<ParsedIntent>(jsonText!, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
				return parsed;
			}
			catch { return null; }
		}
	}
}


