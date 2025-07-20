using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Response;

namespace TourManagement_BE.Service
{
    public class DashboardCustomerService : IDashboardCustomerService
    {
        private readonly MyDBContext _context;
        public DashboardCustomerService(MyDBContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalBookingsAsync(int userId)
        {
            return await _context.Bookings.CountAsync(b => b.UserId == userId && b.IsActive);
        }

        public async Task<int> GetTotalTransactionsAsync(int userId)
        {
            return await _context.Payments.CountAsync(p => p.UserId == userId && p.IsActive);
        }

        public async Task<decimal> GetAverageValueAsync(int userId)
        {
            var bookings = await _context.Bookings.Where(b => b.UserId == userId && b.IsActive && b.TotalPrice.HasValue).ToListAsync();
            if (bookings.Count == 0) return 0;
            return bookings.Average(b => b.TotalPrice ?? 0);
        }

        public async Task<List<RecentBookingResponse>> GetRecentBookingsAsync(int userId, int count = 5)
        {
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId && b.IsActive)
                .OrderByDescending(b => b.BookingDate)
                .Take(count)
                .Include(b => b.Tour)
                .ToListAsync();
            return bookings.Select(b => new RecentBookingResponse
            {
                BookingId = b.BookingId,
                TourTitle = b.Tour.Title,
                //SelectedDepartureDate = b.SelectedDepartureDate,
                BookingDate = b.BookingDate,
                TotalPrice = b.TotalPrice,
                BookingStatus = b.BookingStatus,
                PaymentStatus = b.PaymentStatus,
                NumberOfAdults = b.NumberOfAdults,
                NumberOfChildren = b.NumberOfChildren,
                NumberOfInfants = b.NumberOfInfants
            }).ToList();
        }

        public async Task<List<RecentInvoiceResponse>> GetRecentInvoicesAsync(int userId, int count = 5)
        {
            var payments = await _context.Payments
                .Where(p => p.UserId == userId && p.IsActive)
                .OrderByDescending(p => p.PaymentDate)
                .Take(count)
                .Include(p => p.Booking)
                .ThenInclude(b => b.Tour)
                .ToListAsync();
            return payments.Select(p => new RecentInvoiceResponse
            {
                PaymentId = p.PaymentId,
                BookingId = p.BookingId,
                TourTitle = p.Booking?.Tour?.Title ?? string.Empty,
                Amount = p.Amount,
                AmountPaid = p.AmountPaid,
                PaymentMethod = p.PaymentMethod,
                PaymentStatus = p.PaymentStatus,
                PaymentDate = p.PaymentDate,
                PaymentReference = p.PaymentReference
            }).ToList();
        }
    }
}
 