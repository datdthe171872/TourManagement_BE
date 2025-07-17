using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;

namespace TourManagement_BE.Service
{
    public class DashboardOperatorService : IDashboardOperatorService
    {
        private readonly MyDBContext _context;
        public DashboardOperatorService(MyDBContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalToursAsync(int userId)
        {
            var tourOperator = await _context.TourOperators.FirstOrDefaultAsync(to => to.UserId == userId && to.IsActive);
            if (tourOperator == null) return 0;
            return await _context.Tours.CountAsync(t => t.TourOperatorId == tourOperator.TourOperatorId && t.IsActive);
        }

        public async Task<int> GetTotalBookingsAsync(int userId)
        {
            var tourOperator = await _context.TourOperators.FirstOrDefaultAsync(to => to.UserId == userId && to.IsActive);
            if (tourOperator == null) return 0;
            var tourIds = await _context.Tours.Where(t => t.TourOperatorId == tourOperator.TourOperatorId && t.IsActive).Select(t => t.TourId).ToListAsync();
            return await _context.Bookings.CountAsync(b => tourIds.Contains(b.TourId) && b.IsActive);
        }

        public async Task<decimal> GetTotalEarningsAsync(int userId)
        {
            var tourOperator = await _context.TourOperators.FirstOrDefaultAsync(to => to.UserId == userId && to.IsActive);
            if (tourOperator == null) return 0;
            var tourIds = await _context.Tours.Where(t => t.TourOperatorId == tourOperator.TourOperatorId && t.IsActive).Select(t => t.TourId).ToListAsync();
            var bookings = await _context.Bookings.Where(b => tourIds.Contains(b.TourId) && b.IsActive && b.TotalPrice.HasValue).ToListAsync();
            return bookings.Sum(b => b.TotalPrice ?? 0);
        }

        public async Task<int> GetTotalFeedbacksAsync(int userId)
        {
            var tourOperator = await _context.TourOperators.FirstOrDefaultAsync(to => to.UserId == userId && to.IsActive);
            if (tourOperator == null) return 0;
            var tourIds = await _context.Tours.Where(t => t.TourOperatorId == tourOperator.TourOperatorId && t.IsActive).Select(t => t.TourId).ToListAsync();
            return await _context.TourRatings.CountAsync(tr => tourIds.Contains(tr.TourId) && tr.IsActive);
        }

        public async Task<List<object>> GetLatestInvoicesAsync(int userId, int count = 5)
        {
            var tourOperator = await _context.TourOperators.FirstOrDefaultAsync(to => to.UserId == userId && to.IsActive);
            if (tourOperator == null) return new List<object>();

            var tourIds = await _context.Tours.Where(t => t.TourOperatorId == tourOperator.TourOperatorId && t.IsActive).Select(t => t.TourId).ToListAsync();
            var bookingIds = await _context.Bookings.Where(b => tourIds.Contains(b.TourId) && b.IsActive).Select(b => b.BookingId).ToListAsync();

            var payments = await _context.Payments
                .Where(p => bookingIds.Contains(p.BookingId) && p.IsActive)
                .OrderByDescending(p => p.PaymentDate)
                .Take(count)
                .Include(p => p.Booking)
                .ThenInclude(b => b.Tour)
                .Include(p => p.User)
                .Select(p => new
                {
                    PaymentId = p.PaymentId,
                    BookingId = p.BookingId,
                    TourTitle = p.Booking.Tour.Title,
                    CustomerName = p.User.UserName ?? p.User.Email,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus,
                    PaymentDate = p.PaymentDate
                })
                .ToListAsync();

            return payments.Cast<object>().ToList();
        }
    }
}