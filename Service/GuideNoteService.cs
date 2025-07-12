using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Service
{
    public class GuideNoteService : IGuideNoteService
    {
        private readonly MyDBContext _context;
        public GuideNoteService(MyDBContext context)
        {
            _context = context;
        }

        public async Task<List<GuideNoteResponse>> GetNotesByGuideUserIdAsync(int userId)
        {
            // Lấy TourGuideId từ UserId
            var guide = await _context.TourGuides.FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            if (guide == null) return new List<GuideNoteResponse>();
            // Lấy tất cả assignment của guide
            var assignmentIds = await _context.TourGuideAssignments
                .Where(a => a.TourGuideId == guide.TourGuideId && a.IsActive)
                .Select(a => a.Id).ToListAsync();
            // Lấy note theo assignment
            var notes = await _context.GuideNotes
                .Where(n => assignmentIds.Contains(n.AssignmentId) && n.IsActive)
                .Include(n => n.GuideNoteMedia)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return notes.Select(n => new GuideNoteResponse
            {
                NoteId = n.NoteId,
                AssignmentId = n.AssignmentId,
                Title = n.Title,
                Content = n.Content,
                CreatedAt = n.CreatedAt,
                MediaUrls = n.GuideNoteMedia.Where(m => m.IsActive).Select(m => m.MediaUrl).ToList()
            }).ToList();
        }

        public async Task CreateNoteAsync(int userId, CreateGuideNoteRequest request)
        {
            var guide = await _context.TourGuides.FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            if (guide == null) throw new Exception("Guide not found");
            // Kiểm tra assignment có thuộc guide không
            var assignment = await _context.TourGuideAssignments.FirstOrDefaultAsync(a => a.Id == request.AssignmentId && a.TourGuideId == guide.TourGuideId && a.IsActive);
            if (assignment == null) throw new Exception("Assignment not found");
            var note = new GuideNote
            {
                AssignmentId = request.AssignmentId,
                Title = request.Title,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _context.GuideNotes.Add(note);
            await _context.SaveChangesAsync();
            // Thêm media nếu có
            if (request.MediaUrls != null && request.MediaUrls.Count > 0)
            {
                foreach (var url in request.MediaUrls)
                {
                    var media = new GuideNoteMedia
                    {
                        NoteId = note.NoteId,
                        MediaUrl = url,
                        UploadedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    _context.GuideNoteMedia.Add(media);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateNoteAsync(int userId, int noteId, UpdateGuideNoteRequest request)
        {
            var guide = await _context.TourGuides.FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            if (guide == null) throw new Exception("Guide not found");
            // Lấy note và kiểm tra quyền
            var note = await _context.GuideNotes.Include(n => n.Assignment).FirstOrDefaultAsync(n => n.NoteId == noteId && n.IsActive);
            if (note == null) throw new Exception("Note not found");
            if (note.Assignment.TourGuideId != guide.TourGuideId) throw new Exception("Not your note");
            note.Title = request.Title;
            note.Content = request.Content;
            // Xoá media cũ
            var oldMedia = await _context.GuideNoteMedia.Where(m => m.NoteId == noteId && m.IsActive).ToListAsync();
            foreach (var m in oldMedia) { m.IsActive = false; }
            // Thêm media mới
            if (request.MediaUrls != null && request.MediaUrls.Count > 0)
            {
                foreach (var url in request.MediaUrls)
                {
                    var media = new GuideNoteMedia
                    {
                        NoteId = note.NoteId,
                        MediaUrl = url,
                        UploadedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    _context.GuideNoteMedia.Add(media);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNoteAsync(int userId, int noteId)
        {
            var guide = await _context.TourGuides.FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            if (guide == null) throw new Exception("Guide not found");
            var note = await _context.GuideNotes.Include(n => n.Assignment).FirstOrDefaultAsync(n => n.NoteId == noteId && n.IsActive);
            if (note == null) throw new Exception("Note not found");
            if (note.Assignment.TourGuideId != guide.TourGuideId) throw new Exception("Not your note");
            note.IsActive = false;
            // Xoá media liên quan
            var medias = await _context.GuideNoteMedia.Where(m => m.NoteId == noteId && m.IsActive).ToListAsync();
            foreach (var m in medias) { m.IsActive = false; }
            await _context.SaveChangesAsync();
        }
    }
} 