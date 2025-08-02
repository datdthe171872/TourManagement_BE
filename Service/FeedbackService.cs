using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace TourManagement_BE.Service;

public class FeedbackService : IFeedbackService
{
    private readonly MyDBContext _context;
    private readonly INotificationService _notificationService;
    private readonly Cloudinary _cloudinary;

    public FeedbackService(MyDBContext context, INotificationService notificationService, Cloudinary cloudinary)
    {
        _context = context;
        _notificationService = notificationService;
        _cloudinary = cloudinary;
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
                TourName = tr.Tour.Title,
                UserName = tr.User.UserName,
                UserEmail = tr.User.Email
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

    public async Task<FeedbackListResponse> GetUserFeedbacksAsync(int userId)
    {
        var query = _context.TourRatings
            .Include(tr => tr.Tour)
            .Include(tr => tr.User)
            .Where(tr => tr.UserId == userId && tr.IsActive);

        var totalCount = await query.CountAsync();

        var feedbacks = await query
            .OrderByDescending(tr => tr.CreatedAt)
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
                TourName = tr.Tour.Title,
                UserName = tr.User.UserName,
                UserEmail = tr.User.Email
            })
            .ToListAsync();

        return new FeedbackListResponse
        {
            Feedbacks = feedbacks,
            TotalCount = totalCount,
            PageNumber = 1,
            PageSize = totalCount,
            TotalPages = 1
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
                TourName = tr.Tour.Title,
                UserName = tr.User.UserName,
                UserEmail = tr.User.Email
            })
            .FirstOrDefaultAsync();

        return feedback;
    }

    public async Task<FeedbackResponse> CreateFeedbackAsync(CreateFeedbackRequest request, int userId)
    {
        // Check if tour exists
        var tour = await _context.Tours.FindAsync(request.TourId);
        if (tour == null)
        {
            throw new InvalidOperationException("Tour không tồn tại");
        }

        // Check if user exists
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User không tồn tại");
        }

        // Check if user has already rated this tour
        var existingRating = await _context.TourRatings
            .FirstOrDefaultAsync(tr => tr.TourId == request.TourId && tr.UserId == userId && tr.IsActive);
        
        if (existingRating != null)
        {
            throw new InvalidOperationException("Bạn đã đánh giá tour này rồi");
        }

        string? mediaUrl = null;

        // Upload image if provided
        if (request.ImageFile != null && request.ImageFile.Length > 0)
        {
            // Validate file size (max 10MB)
            if (request.ImageFile.Length > 10 * 1024 * 1024)
            {
                throw new InvalidOperationException("Kích thước file không được vượt quá 10MB");
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(request.ImageFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif)");
            }

            // Upload to Cloudinary
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(request.ImageFile.FileName, request.ImageFile.OpenReadStream()),
                Folder = "ProjectSEP490/Feedback/Images"
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            mediaUrl = uploadResult.SecureUrl.ToString();
        }

        var feedback = new Data.Models.TourRating
        {
            TourId = request.TourId,
            UserId = userId,
            Rating = request.Rating,
            Comment = request.Comment,
            MediaUrl = mediaUrl,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.TourRatings.Add(feedback);
        await _context.SaveChangesAsync();

        // Tạo notification khi tạo feedback thành công
        await _notificationService.CreateFeedbackNotificationAsync(userId, feedback.RatingId);

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
            TourName = tour.Title,
            UserName = user.UserName,
            UserEmail = user.Email
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
            TourName = feedback.Tour.Title,
            UserName = feedback.User.UserName,
            UserEmail = feedback.User.Email
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

    public async Task<bool> UpdateFeedbackStatusAsync(int ratingId, bool isActive)
    {
        var feedback = await _context.TourRatings
            .Include(tr => tr.User)
            .FirstOrDefaultAsync(tr => tr.RatingId == ratingId);

        if (feedback == null)
        {
            return false;
        }

        var previousStatus = feedback.IsActive;
        feedback.IsActive = isActive;
        await _context.SaveChangesAsync();

        // Send notification if feedback is deactivated
        if (previousStatus && !isActive)
        {
            await _notificationService.CreateFeedbackViolationNotificationAsync(feedback.UserId, feedback.RatingId);
        }

        return true;
    }

    public async Task<FeedbackListResponse> GetAdminFeedbacksAsync(AdminFeedbackSearchRequest request)
    {
        var query = _context.TourRatings
            .Include(tr => tr.Tour)
            .Include(tr => tr.User)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            query = query.Where(tr => tr.User != null && 
                                    tr.User.UserName != null && 
                                    tr.User.UserName.Contains(request.Username));
        }

        if (request.RatingId.HasValue)
        {
            query = query.Where(tr => tr.RatingId == request.RatingId.Value);
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
                TourName = tr.Tour.Title,
                UserName = tr.User.UserName,
                UserEmail = tr.User.Email
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

    public async Task<FeedbackListResponse> GetTourOperatorFeedbacksAsync(int tourOperatorId, TourOperatorFeedbackSearchRequest request)
    {
        var query = _context.TourRatings
            .Include(tr => tr.Tour)
            .Include(tr => tr.User)
            .Where(tr => tr.Tour.TourOperatorId == tourOperatorId);

        // Apply filters
        if (request.RatingId.HasValue)
        {
            query = query.Where(tr => tr.RatingId == request.RatingId.Value);
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
                TourName = tr.Tour.Title,
                UserName = tr.User.UserName,
                UserEmail = tr.User.Email
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

    public async Task<bool> ReportFeedbackAsync(int tourOperatorId, ReportFeedbackRequest request)
    {
        // Verify that the feedback exists and belongs to a tour of this tour operator
        var feedback = await _context.TourRatings
            .Include(tr => tr.Tour)
            .FirstOrDefaultAsync(tr => tr.RatingId == request.RatingId && 
                                     tr.Tour.TourOperatorId == tourOperatorId);

        if (feedback == null)
        {
            return false;
        }

        // Get admin users to send notification
        var adminUsers = await _context.Users
            .Include(u => u.Role)
            .Where(u => u.Role.RoleName == "Admin" && u.IsActive)
            .ToListAsync();

        // Send notification to all admin users
        foreach (var admin in adminUsers)
        {
            await _notificationService.CreateFeedbackReportNotificationAsync(
                admin.UserId, 
                request.RatingId, 
                request.Reason,
                tourOperatorId
            );
        }

        return true;
    }
} 