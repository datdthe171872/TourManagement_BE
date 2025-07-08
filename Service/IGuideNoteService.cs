using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service
{
    public interface IGuideNoteService
    {
        Task<List<GuideNoteResponse>> GetNotesByGuideUserIdAsync(int userId);
        Task CreateNoteAsync(int userId, CreateGuideNoteRequest request);
        Task UpdateNoteAsync(int userId, int noteId, UpdateGuideNoteRequest request);
        Task DeleteNoteAsync(int userId, int noteId);
    }
} 