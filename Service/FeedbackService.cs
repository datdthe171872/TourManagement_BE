using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service;

public class FeedbackService : IFeedbackService
{
    private readonly MyDBContext _context;

    public FeedbackService(MyDBContext context)
    {
        _context = context;
    }

    public async Task<FeedbackListResponse> GetFeedbacksAsync(FeedbackSearchRequest request)
    {
        var query = _context.TourRatings
            .Include(tr => tr.Tour)
            .Include(tr => tr.User)
            .Where(tr => tr.IsActive);

        // Apply filters
        if (request.TourId.HasValue)
        {
            query = query.Where(tr => tr.TourId == request.TourId.Value);
        }

        if (request.UserId.HasValue)
        {
            query = query.Where(tr => tr.UserId == request.UserId.Value);
        }

        if (request.Rating.HasValue)
        {
            query = query.Where(tr => tr.Rating == request.Rating.Value);
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var feedbacks = await query
            .OrderByDescending(tr => tr.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(tr => new FeedbackResponse
            {
                RatingId = tr.RatingId,
                TourId = tr.TourId,
                UserId = tr.UserId,
                Rating = tr.Rating,
                Comment = tr.Comment,
                MediaUrl = tr.MediaUrl,
                CreatedAt = tr.CreatedAt,
                IsActive = tr.IsActive,
                
            })
            .ToListAsync();

        return new FeedbackListResponse
        {
            Feedbacks = feedbacks,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = totalPages
        };
    }

    public async Task<FeedbackResponse?> GetFeedbackDetailAsync(int id)
    {
        var feedback = await _context.TourRatings
            .Include(tr => tr.Tour)
            .Include(tr => tr.User)
            .Where(tr => tr.RatingId == id && tr.IsActive)
            .Select(tr => new FeedbackResponse
            {
                RatingId = tr.RatingId,
                TourId = tr.TourId,
                UserId = tr.UserId,
                Rating = tr.Rating,
                Comment = tr.Comment,
                MediaUrl = tr.MediaUrl,
                CreatedAt = tr.CreatedAt,
                IsActive = tr.IsActive,
               
            })
            .FirstOrDefaultAsync();

        return feedback;
    }

    public async Task<FeedbackResponse> CreateFeedbackAsync(CreateFeedbackRequest request)
    {
        // Check if tour exists
        var tour = await _context.Tours.FindAsync(request.TourId);
        if (tour == null)
        {
            throw new InvalidOperationException("Tour không tồn tại");
        }

        // Check if user exists
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
        {
            throw new InvalidOperationException("User không tồn tại");
        }

        // Check if user has already rated this tour
        var existingRating = await _context.TourRatings
            .FirstOrDefaultAsync(tr => tr.TourId == request.TourId && tr.UserId == request.UserId && tr.IsActive);
        
        if (existingRating != null)
        {
            throw new InvalidOperationException("User đã đánh giá tour này rồi");
        }

        var feedback = new Data.Models.TourRating
        {
            TourId = request.TourId,
            UserId = request.UserId,
            Rating = request.Rating,
            Comment = request.Comment,
            MediaUrl = request.MediaUrl,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.TourRatings.Add(feedback);
        await _context.SaveChangesAsync();

        return new FeedbackResponse
        {
            RatingId = feedback.RatingId,
            TourId = feedback.TourId,
            UserId = feedback.UserId,
            Rating = feedback.Rating,
            Comment = feedback.Comment,
            MediaUrl = feedback.MediaUrl,
            CreatedAt = feedback.CreatedAt,
            IsActive = feedback.IsActive,
            
        };
    }

    public async Task<FeedbackResponse?> UpdateFeedbackAsync(int id, UpdateFeedbackRequest request)
    {
        var feedback = await _context.TourRatings
            .Include(tr => tr.Tour)
            .Include(tr => tr.User)
            .FirstOrDefaultAsync(tr => tr.RatingId == id && tr.IsActive);

        if (feedback == null)
        {
            return null;
        }

        // Update properties
        if (request.Rating.HasValue)
        {
            feedback.Rating = request.Rating.Value;
        }

        if (request.Comment != null)
        {
            feedback.Comment = request.Comment;
        }

        if (request.MediaUrl != null)
        {
            feedback.MediaUrl = request.MediaUrl;
        }

        await _context.SaveChangesAsync();

        return new FeedbackResponse
        {
            RatingId = feedback.RatingId,
            TourId = feedback.TourId,
            UserId = feedback.UserId,
            Rating = feedback.Rating,
            Comment = feedback.Comment,
            MediaUrl = feedback.MediaUrl,
            CreatedAt = feedback.CreatedAt,
            IsActive = feedback.IsActive,
           
        };
    }

    public async Task<bool> SoftDeleteFeedbackAsync(int id)
    {
        var feedback = await _context.TourRatings
            .FirstOrDefaultAsync(tr => tr.RatingId == id && tr.IsActive);

        if (feedback == null)
        {
            return false;
        }

        feedback.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }
} 