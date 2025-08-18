using System.Threading.Tasks;
using TourManagement_BE.Data.DTO.Request.Chat;

namespace TourManagement_BE.Service
{
	public interface IGeminiClient
	{
		Task<ParsedCriteria?> ExtractCriteriaAsync(string userMessage);
		Task<ParsedIntent?> ExtractIntentAsync(string userMessage);
	}
}


