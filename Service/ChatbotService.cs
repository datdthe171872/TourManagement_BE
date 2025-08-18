using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.Chat;
using TourManagement_BE.Data.DTO.Response.Chat;
using TourManagement_BE.Data.DTO.Request.Chat;
using TourManagement_BE.Data.DTO.Request;
using System.Text.RegularExpressions;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service
{
	public class ChatbotService : IChatbotService
	{
		private readonly MyDBContext _db;
		private readonly IGeminiClient _gemini;
        private readonly ITourComparisonService _comparisonService;

		public ChatbotService(MyDBContext db, IGeminiClient gemini, ITourComparisonService comparisonService)
		{
			_db = db;
			_gemini = gemini;
            _comparisonService = comparisonService;
		}

		public async Task<ChatResponse> ProcessMessageAsync(ChatRequest request)
		{
			var message = (request.Message ?? string.Empty).ToLowerInvariant();

			// 0) Nhận diện intent bằng heuristic trước (ưu tiên)
			var localIntent = DetectIntentHeuristics(message);
			ParsedIntent? intent = null;
			string resolvedIntent;
			if (localIntent != null)
			{
				intent = localIntent;
				resolvedIntent = intent.Intent;
			}
			else
			{
				// 0.1) Thử Gemini nếu không match heuristic
				try { intent = await _gemini.ExtractIntentAsync(request.Message ?? string.Empty); } catch { }
				resolvedIntent = intent?.Intent ?? "suggest";
			}

			// Branch theo intent
			if (resolvedIntent == "tour_detail" && intent?.TourId is int tdId)
			{
				var detail = await GetTourDetailAsync(tdId);
				return new ChatResponse
				{
					Intent = resolvedIntent,
					Reply = detail == null ? "Không tìm thấy tour bạn yêu cầu." : $"Thông tin tour {detail.Title}",
					Payload = detail
				};
			}

			if (resolvedIntent == "compare_tours" && intent?.Tour1Id is int c1 && intent?.Tour2Id is int c2)
			{
				var compare = await CompareTwoToursAsync(c1, c2);
				string advice = BuildAdviceFromComparison(compare);
				return new ChatResponse { Intent = resolvedIntent, Reply = advice, Payload = compare };
			}

			if (resolvedIntent == "plan_booking")
			{
				if ((request.UserId ?? 0) <= 0)
					return new ChatResponse { Intent = resolvedIntent, Reply = "Bạn cần đăng nhập để đặt tour." };
				var plan = intent?.Plan ?? new BookingPlan();
				// Heuristic bổ sung: nếu người dùng nói "gần nhất" → chọn DepartureDate gần nhất của tour nếu chưa có
				if (plan.TourId.HasValue && !plan.DepartureDateId.HasValue && (message.Contains("gần nhất") || message.Contains("gan nhat") || message.Contains("sớm nhất") || message.Contains("som nhat")))
				{
					var depNearest = await _db.DepartureDates
						.Where(d => d.TourId == plan.TourId.Value && d.IsActive && d.DepartureDate1 >= DateTime.UtcNow)
						.OrderBy(d => d.DepartureDate1)
						.Select(d => d.Id)
						.FirstOrDefaultAsync();
					if (depNearest != 0) plan.DepartureDateId = depNearest;
				}
				// Heuristic bổ sung: bắt số lượng người nếu câu chứa "x người lớn", "y trẻ em", "z em bé"
				var mAdults = System.Text.RegularExpressions.Regex.Match(message, @"(\d+)\s*(người lớn|nguoi lon)");
				if (mAdults.Success && int.TryParse(mAdults.Groups[1].Value, out var ad)) plan.NumberOfAdults = ad;
				var mChildren = System.Text.RegularExpressions.Regex.Match(message, @"(\d+)\s*(trẻ em|tre em)");
				if (mChildren.Success && int.TryParse(mChildren.Groups[1].Value, out var ch)) plan.NumberOfChildren = ch;
				var mInfants = System.Text.RegularExpressions.Regex.Match(message, @"(\d+)\s*(em bé|embe|em be|sơ sinh|so sinh)");
				if (mInfants.Success && int.TryParse(mInfants.Groups[1].Value, out var inf)) plan.NumberOfInfants = inf;
				var uid = request.UserId!.Value;
				var res = await PlanOrCreateBookingAsync(plan, uid);
				return new ChatResponse { Intent = resolvedIntent, Reply = "Kế hoạch đặt tour", Payload = res };
			}

			if (resolvedIntent == "help")
			{
				var guide = "Cách đặt tour qua chatbot:\n" +
					"1) Cho biết tour muốn đặt (ví dụ: mã tour hoặc tên tour).\n" +
					"2) Chọn ngày khởi hành (chat ngày hoặc chọn từ gợi ý).\n" +
					"3) Nhập số người (người lớn/trẻ em/em bé).\n" +
					"4) Chatbot sẽ hỏi xác nhận. Trả lời đồng ý để tạo booking.\n" +
					"5) Kiểm tra trạng thái: gửi ‘booking [ID] tới đâu rồi?’.\n" +
					"(Lưu ý: chưa kích hoạt thanh toán trong chatbot theo yêu cầu.)";
				return new ChatResponse { Intent = resolvedIntent, Reply = guide };
			}

			if (resolvedIntent == "booking_status" && intent?.BookingId is int bId)
			{
				if ((request.UserId ?? 0) <= 0)
					return new ChatResponse { Intent = resolvedIntent, Reply = "Bạn cần đăng nhập để xem trạng thái booking." };
				var uid = request.UserId!.Value;
				var status = await GetBookingStatusAsync(bId, uid);
				return new ChatResponse { Intent = resolvedIntent, Reply = status == null ? "Không tìm thấy booking" : "Trạng thái booking", Payload = status };
			}

			if (resolvedIntent == "tour_status")
			{
				var title = intent?.TourTitle?.Trim();
				var opName = intent?.TourOperatorName?.Trim();
				IQueryable<Data.Models.Tour> q = _db.Tours.Include(t => t.DepartureDates);
				if (!string.IsNullOrEmpty(title))
				{
					var tl = title.ToLower();
					// Ưu tiên so khớp chính xác (không phân biệt hoa thường), nếu không có thì fallback Contains
					q = q.Where(t => t.Title.ToLower() == tl || t.Title.Contains(title));
				}
				if (!string.IsNullOrEmpty(opName))
				{
					q = q.Include(t => t.TourOperator).Where(t => t.TourOperator.CompanyName.Contains(opName));
				}
				var tour = await q.OrderByDescending(t => t.CreatedAt).FirstOrDefaultAsync();
				if (tour == null) return new ChatResponse { Intent = resolvedIntent, Reply = "Không tìm thấy tour phù hợp" };
				var upcoming = tour.DepartureDates.Where(d => d.IsActive && d.DepartureDate1 >= DateTime.UtcNow)
					.OrderBy(d => d.DepartureDate1).ToList();
				var payload = new
				{
					tourId = tour.TourId,
					title = tour.Title,
					status = tour.TourStatus ?? (tour.IsActive ? "Active" : "Inactive"),
					upcomingCount = upcoming.Count,
					nearest = upcoming.Select(d => d.DepartureDate1).FirstOrDefault()
				};
				return new ChatResponse { Intent = resolvedIntent, Reply = $"Tour '{tour.Title}' đang {(tour.IsActive ? "mở" : "đóng")}.", Payload = payload };
			}

			// 1) Thử dùng Gemini để trích xuất tiêu chí cho intent gợi ý
			ParsedCriteria? criteria = null;
			try { criteria = await _gemini.ExtractCriteriaAsync(request.Message ?? string.Empty); } catch { }

			string? destination = criteria?.Destination?.ToLowerInvariant();
			int? durationDays = criteria?.DurationDays;
			decimal? budgetMin = criteria?.BudgetMin;
			decimal? budgetMax = criteria?.BudgetMax;

			// 2) Fallback rule-based nhẹ nếu Gemini không có tiêu chí
			// Tìm theo một số từ khóa đơn giản: "đà nẵng", "nha trang", số ngày, ngân sách
			if (string.IsNullOrWhiteSpace(destination)) destination = null;
			if (!durationDays.HasValue) durationDays = null;
			if (!budgetMin.HasValue && !budgetMax.HasValue)
			{
				decimal? budget = null;

			if (message.Contains("đà nẵng")) destination = "đà nẵng";
			if (message.Contains("nha trang")) destination = "nha trang";
			if (message.Contains("đà lạt")) destination = "đà lạt";
			if (message.Contains("phú quốc")) destination = "phú quốc";

			// duration: bắt các mẫu "2n", "3 ngày", "4 ngay"
			for (int d = 2; d <= 10; d++)
			{
				if (message.Contains($"{d}n") || message.Contains($"{d} n") || message.Contains($"{d} ngày") || message.Contains($"{d} ngay"))
				{
					durationDays = d;
					break;
				}
			}

			// budget: tìm số tiền dạng xxxk, x.x tr, hoặc số nguyên (đồng)
			try
			{
				var tokens = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				foreach (var t in tokens)
				{
					var token = t.Replace(".", "").Replace(",", "");
					if (token.EndsWith("k"))
					{
						if (decimal.TryParse(token.TrimEnd('k'), out var k)) budget = k * 1000m;
					}
					else if (token.EndsWith("tr") || token.EndsWith("triệu") || token.EndsWith("trieu"))
					{
						var val = token.Replace("triệu", "").Replace("trieu", "").Replace("tr", "");
						if (decimal.TryParse(val, out var tr)) budget = tr * 1_000_000m;
					}
					else if (decimal.TryParse(token, out var v) && v >= 10000)
					{
						budget = v;
					}
				}
			}
			catch { }
			if (budget.HasValue) { budgetMax = budget; }
			}

			var toursQuery = _db.Tours.AsQueryable().Where(t => t.IsActive && (t.TourStatus == null || t.TourStatus == "Active"));

			if (!string.IsNullOrEmpty(destination))
			{
				var destLower = destination.ToLower();
				toursQuery = toursQuery.Where(t => (t.Title != null && t.Title.ToLower().Contains(destLower))
					|| (t.Description != null && t.Description.ToLower().Contains(destLower))
					|| (t.StartPoint != null && t.StartPoint.ToLower().Contains(destLower)));
			}

			if (durationDays.HasValue)
			{
				// DurationInDays là string, thường dạng "3 ngày 2 đêm"; lọc khoảng chứa số ngày
				var durationStr = durationDays.Value.ToString();
				toursQuery = toursQuery.Where(t => t.DurationInDays != null && EF.Functions.Like(t.DurationInDays, $"%{durationStr}%"));
			}

			if (budgetMax.HasValue)
			{
				var bMax = budgetMax.Value;
				toursQuery = toursQuery.Where(t => t.PriceOfAdults <= bMax);
			}
			if (budgetMin.HasValue)
			{
				var bMin = budgetMin.Value;
				toursQuery = toursQuery.Where(t => t.PriceOfAdults >= bMin);
			}

			// Lọc theo điểm xuất phát và phương tiện nếu có
			if (!string.IsNullOrWhiteSpace(criteria?.StartPoint))
			{
				var sp = criteria!.StartPoint!.ToLowerInvariant();
				toursQuery = toursQuery.Where(t => t.StartPoint != null && t.StartPoint.ToLower().Contains(sp));
			}
			if (!string.IsNullOrWhiteSpace(criteria?.Transportation))
			{
				var trans = criteria!.Transportation!.ToLowerInvariant();
				toursQuery = toursQuery.Where(t => t.Transportation != null && t.Transportation.ToLower().Contains(trans));
			}

			// Lọc theo style (suy diễn qua tiêu đề/mô tả nếu DB chưa có trường riêng)
			if (!string.IsNullOrWhiteSpace(criteria?.Style))
			{
				var style = criteria!.Style!.ToLowerInvariant();
				toursQuery = toursQuery.Where(t => (t.Title != null && t.Title.ToLower().Contains(style)) || (t.Description != null && t.Description.ToLower().Contains(style)));
			}

			// Lọc theo khoảng ngày khởi hành qua bảng DepartureDates
			if (criteria?.DateFrom.HasValue == true || criteria?.DateTo.HasValue == true)
			{
				var from = criteria?.DateFrom ?? DateTime.MinValue;
				var to = criteria?.DateTo ?? DateTime.MaxValue;
				toursQuery = toursQuery.Where(t => t.DepartureDates.Any(dd => dd.IsActive && dd.DepartureDate1 >= from && dd.DepartureDate1 <= to));
			}

			// Lọc theo quy mô nhóm dựa trên slot còn trống (xấp xỉ)
			if (criteria?.GroupSize.HasValue == true)
			{
				var need = criteria!.GroupSize!.Value;
				toursQuery = toursQuery.Where(t => (t.MaxSlots - (t.SlotsBooked ?? 0)) >= need);
			}

			// Nếu không có bất kỳ tiêu chí nào → hỏi làm rõ thay vì trả 5 tour mặc định
			bool hasAnyCriteria = !string.IsNullOrEmpty(destination) || durationDays.HasValue || budgetMin.HasValue || budgetMax.HasValue ||
				(!string.IsNullOrWhiteSpace(criteria?.StartPoint)) || (!string.IsNullOrWhiteSpace(criteria?.Transportation)) || (!string.IsNullOrWhiteSpace(criteria?.Style)) ||
				(criteria?.DateFrom.HasValue == true || criteria?.DateTo.HasValue == true) || (criteria?.GroupSize.HasValue == true);
			if (!hasAnyCriteria)
			{
				return new ChatResponse
				{
					Intent = "suggest",
					Reply = "Bạn muốn đi đâu, khoảng mấy ngày và ngân sách tầm bao nhiêu? Mình sẽ gợi ý chính xác hơn.",
					Suggestions = new System.Collections.Generic.List<TourSuggestionItem>()
				};
			}

			var tours = await toursQuery
				.OrderBy(t => t.PriceOfAdults)
				.Take(5)
				.Select(t => new TourSuggestionItem
				{
					TourId = t.TourId,
					Title = t.Title,
					PriceOfAdults = t.PriceOfAdults,
					PriceOfChildren = t.PriceOfChildren,
					PriceOfInfants = t.PriceOfInfants,
					DurationInDays = t.DurationInDays,
					StartPoint = t.StartPoint,
					TourAvartar = t.TourAvartar,
					UpcomingDepartureDates = t.DepartureDates
						.Where(d => d.IsActive && d.DepartureDate1 >= DateTime.UtcNow)
						.OrderBy(d => d.DepartureDate1)
						.Take(5)
						.Select(d => d.DepartureDate1)
						.ToList()
				})
				.ToListAsync();

			var response = new ChatResponse
			{
				Intent = "suggest",
				Reply = BuildReply(destination, durationDays, budgetMin, budgetMax, tours.Count),
				Suggestions = tours
			};

			return response;
		}

		private static string BuildAdviceFromComparison(object compareObj)
		{
			try
			{
				var cmp = compareObj as TourComparisonResponse;
				if (cmp == null) return "Kết quả so sánh 2 tour:";
				var r = cmp.Result;
				// Quy tắc khuyến nghị đơn giản:
				// - Nếu chênh lệch thắng > 1 tiêu chí → khuyên chọn tour thắng
				// - Nếu hòa/nhỉnh nhẹ → xét giá và đánh giá trung bình để khuyên
				if (r.Tour1Wins - r.Tour2Wins >= 2) return $"Nên chọn tour '{cmp.Tour1.Title}' (thắng {r.Tour1Wins}/{r.Tour2Wins + r.Ties + r.Tour1Wins} tiêu chí).";
				if (r.Tour2Wins - r.Tour1Wins >= 2) return $"Nên chọn tour '{cmp.Tour2.Title}' (thắng {r.Tour2Wins}/{r.Tour1Wins + r.Ties + r.Tour2Wins} tiêu chí).";
				// Cân bằng: ưu tiên đánh giá, sau đó giá
				if (cmp.Tour1.AverageRating > cmp.Tour2.AverageRating + 0.3)
					return $"Nên chọn tour '{cmp.Tour1.Title}' (điểm đánh giá cao hơn).";
				if (cmp.Tour2.AverageRating > cmp.Tour1.AverageRating + 0.3)
					return $"Nên chọn tour '{cmp.Tour2.Title}' (điểm đánh giá cao hơn).";
				if (cmp.Tour1.PriceOfAdults + 200000 <= cmp.Tour2.PriceOfAdults)
					return $"Nên chọn tour '{cmp.Tour1.Title}' (giá tốt hơn).";
				if (cmp.Tour2.PriceOfAdults + 200000 <= cmp.Tour1.PriceOfAdults)
					return $"Nên chọn tour '{cmp.Tour2.Title}' (giá tốt hơn).";
				return "Hai tour khá tương đương. Bạn ưu tiên giá rẻ hay đánh giá cao?";
			}
			catch
			{
				return "Kết quả so sánh 2 tour:";
			}
		}

		private ParsedIntent? DetectIntentHeuristics(string message)
		{
			// help
			if (message.Contains("hướng dẫn") || message.Contains("cách") || message.Contains("how to") || message.Contains("help"))
				return new ParsedIntent { Intent = "help" };

			// tour detail: "xem tour 123", "chi tiết tour 123"
			var mDetail = Regex.Match(message, @"(xem|chi tiết|chi tiet)\s*tour\s*(\d+)");
			if (mDetail.Success && int.TryParse(mDetail.Groups[2].Value, out var td))
				return new ParsedIntent { Intent = "tour_detail", TourId = td };

			// compare: "so sánh tour 12 và 34"
			var mCompare = Regex.Match(message, @"so\s*s(á|a)nh.*?(\d+).*?(\d+)");
			if (mCompare.Success && int.TryParse(mCompare.Groups[2].Value, out var c1) && int.TryParse(mCompare.Groups[3].Value, out var c2))
				return new ParsedIntent { Intent = "compare_tours", Tour1Id = c1, Tour2Id = c2 };

			// booking status: "booking 456", "trạng thái booking 456"
			var mStatus = Regex.Match(message, @"booking\s*(\d+)");
			if (mStatus.Success && int.TryParse(mStatus.Groups[1].Value, out var bId))
				return new ParsedIntent { Intent = "booking_status", BookingId = bId };

			// tour status: "trạng thái tour X cho TOY" (heuristic đơn giản theo tên)
			if (message.Contains("trạng thái tour") || message.Contains("trang thai tour"))
			{
				// lấy cụm sau "trạng thái tour"
				var idx = message.IndexOf("trạng thái tour");
				if (idx < 0) idx = message.IndexOf("trang thai tour");
				var tail = idx >= 0 ? message[(idx + "trạng thái tour".Length)..].Trim() : message;
				return new ParsedIntent { Intent = "tour_status", TourTitle = tail };
			}

			// plan booking: "đặt tour", "dat tour", "book tour"
			if (message.Contains("đặt tour") || message.Contains("dat tour") || message.Contains("book tour") || message.Contains("booking"))
				return new ParsedIntent { Intent = "plan_booking", Plan = new BookingPlan() };

			return null;
		}

		public async Task<TourDetail?> GetTourDetailAsync(int tourId)
		{
			var now = DateTime.UtcNow;
			var tour = await _db.Tours
				.Include(t => t.DepartureDates.Where(d => d.IsActive && d.DepartureDate1 >= now))
				.FirstOrDefaultAsync(t => t.TourId == tourId && t.IsActive);
			if (tour == null) return null;
			return new TourDetail
			{
				TourId = tour.TourId,
				Title = tour.Title,
				Description = tour.Description,
				PriceOfAdults = tour.PriceOfAdults,
				DurationInDays = tour.DurationInDays,
				StartPoint = tour.StartPoint,
				Transportation = tour.Transportation,
				TourAvartar = tour.TourAvartar,
				UpcomingDepartureDates = tour.DepartureDates
					.OrderBy(d => d.DepartureDate1)
					.Take(10)
					.Select(d => d.DepartureDate1)
					.ToList()
			};
		}

		public async Task<object> CompareTwoToursAsync(int tour1Id, int tour2Id)
		{
			var request = new TourComparisonRequest { Tour1Id = tour1Id, Tour2Id = tour2Id };
			var result = await _comparisonService.CompareToursAsync(request);
			return result;
		}

		public async Task<object> PlanOrCreateBookingAsync(BookingPlan plan, int userId)
		{
			// Nếu chưa confirm, trả về gợi ý/thiếu thông tin
			if (!plan.TourId.HasValue)
				return new { need = "tourId", message = "Bạn muốn đặt tour nào? Vui lòng cung cấp mã tour." };
			if (!plan.DepartureDateId.HasValue)
				return new { need = "departureDateId", message = "Bạn muốn khởi hành ngày nào?" };
			if (!plan.NumberOfAdults.HasValue || plan.NumberOfAdults.Value <= 0)
				return new { need = "numberOfAdults", message = "Bạn đi bao nhiêu người lớn?" };
			if (!plan.NumberOfChildren.HasValue) plan.NumberOfChildren = 0;
			if (!plan.NumberOfInfants.HasValue) plan.NumberOfInfants = 0;

			if (!plan.Confirm)
			{
				return new
				{
					confirm = true,
					message = $"Xác nhận đặt tour {plan.TourId} ngày {plan.DepartureDateId} cho {plan.NumberOfAdults}+{plan.NumberOfChildren}+{plan.NumberOfInfants}?"
				};
			}

			// Tạo booking trực tiếp qua DbContext (để không phụ thuộc vào controller); đảm bảo logic cơ bản
			var dep = await _db.DepartureDates.FirstOrDefaultAsync(d => d.Id == plan.DepartureDateId && d.TourId == plan.TourId && d.IsActive);
			if (dep == null) return new { error = "Ngày khởi hành không hợp lệ cho tour này" };
			var tour = await _db.Tours.FirstOrDefaultAsync(t => t.TourId == plan.TourId && t.IsActive);
			if (tour == null) return new { error = "Tour không tồn tại" };

			var totalPeople = plan.NumberOfAdults!.Value + plan.NumberOfChildren!.Value + plan.NumberOfInfants!.Value;
			var available = tour.MaxSlots - (tour.SlotsBooked ?? 0);
			if (totalPeople <= 0) return new { error = "Tổng số người phải lớn hơn 0" };
			if (available < totalPeople) return new { error = "Không đủ chỗ cho số lượng yêu cầu" };

			var booking = new Data.Models.Booking
			{
				TourId = plan.TourId.Value,
				DepartureDateId = plan.DepartureDateId.Value,
				UserId = userId,
				NumberOfAdults = plan.NumberOfAdults.Value,
				NumberOfChildren = plan.NumberOfChildren.Value,
				NumberOfInfants = plan.NumberOfInfants.Value,
				BookingDate = DateTime.UtcNow,
				BookingStatus = "Pending",
				PaymentStatus = "Pending",
				IsActive = true,
				TotalPrice = plan.NumberOfAdults.Value * tour.PriceOfAdults
					+ plan.NumberOfChildren.Value * tour.PriceOfChildren
					+ plan.NumberOfInfants.Value * tour.PriceOfInfants
			};
			_db.Bookings.Add(booking);
			await _db.SaveChangesAsync();

			return new { success = true, bookingId = booking.BookingId, totalPrice = booking.TotalPrice };
		}

		public async Task<object?> GetBookingStatusAsync(int bookingId, int userId)
		{
			var booking = await _db.Bookings.Include(b => b.Payments)
				.FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == userId && b.IsActive);
			if (booking == null) return null;
			return new
			{
				booking.BookingId,
				booking.BookingStatus,
				booking.PaymentStatus,
				TotalPaid = booking.Payments?.Where(p => p.IsActive).Sum(p => p.AmountPaid) ?? 0m,
				booking.TotalPrice
			};
		}

		private static string BuildReply(string? destination, int? durationDays, decimal? budgetMin, decimal? budgetMax, int count)
		{
			if (count == 0)
			{
				return "Mình chưa tìm thấy tour phù hợp theo tiêu chí. Bạn có thể cho mình biết thêm điểm đến, số ngày, và ngân sách dự kiến không?";
			}

			var parts = new System.Collections.Generic.List<string>();
			if (!string.IsNullOrEmpty(destination)) parts.Add($"điểm đến '{destination}'");
			if (durationDays.HasValue) parts.Add($"khoảng {durationDays} ngày");
			if (budgetMin.HasValue || budgetMax.HasValue)
			{
				if (budgetMin.HasValue && budgetMax.HasValue) parts.Add($"ngân sách {budgetMin.Value:N0}–{budgetMax.Value:N0}đ");
				else if (budgetMax.HasValue) parts.Add($"ngân sách tối đa {budgetMax.Value:N0}đ");
				else parts.Add($"ngân sách tối thiểu {budgetMin!.Value:N0}đ");
			}
			var cond = parts.Count > 0 ? $" theo {string.Join(", ", parts)}" : string.Empty;
			return $"Mình gợi ý {count} tour{cond}. Bạn muốn xem chi tiết tour nào, hoặc cần hỗ trợ đặt tour không?";
		}
	}
}


