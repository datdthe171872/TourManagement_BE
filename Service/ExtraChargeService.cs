using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Data.Models;

namespace TourManagement_BE.Service
{
    public class ExtraChargeService : IExtraChargeService
    {
        private readonly MyDBContext _context;
        public ExtraChargeService(MyDBContext context)
        {
            _context = context;
        }

        public async Task<ExtraChargeListResponse> GetAllAsync(bool? isActive = null)
        {
            var query = _context.ExtraCharges.AsQueryable();
            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive.Value);
            var list = await query.ToListAsync();
            return new ExtraChargeListResponse
            {
                ExtraCharges = list.Select(x => new ExtraChargeResponse
                {
                    ExtraChargeId = x.ExtraChargeId,
                    Name = x.Name,
                    Description = x.Description,
                    Amount = x.Amount,
                    IsActive = x.IsActive
                }).ToList()
            };
        }

        public async Task<ExtraChargeResponse> CreateAsync(CreateExtraChargeRequest request)
        {
            var entity = new ExtraCharge
            {
                Name = request.Name,
                Description = request.Description,
                Amount = request.Amount,
                IsActive = true
            };
            _context.ExtraCharges.Add(entity);
            await _context.SaveChangesAsync();
            return new ExtraChargeResponse
            {
                ExtraChargeId = entity.ExtraChargeId,
                Name = entity.Name,
                Description = entity.Description,
                Amount = entity.Amount,
                IsActive = entity.IsActive
            };
        }

        public async Task<ExtraChargeResponse> UpdateAsync(UpdateExtraChargeRequest request)
        {
            var entity = await _context.ExtraCharges.FirstOrDefaultAsync(x => x.ExtraChargeId == request.ExtraChargeId);
            if (entity == null) return null;
            if (request.Name != null) entity.Name = request.Name;
            if (request.Description != null) entity.Description = request.Description;
            if (request.Amount.HasValue) entity.Amount = request.Amount.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
            await _context.SaveChangesAsync();
            return new ExtraChargeResponse
            {
                ExtraChargeId = entity.ExtraChargeId,
                Name = entity.Name,
                Description = entity.Description,
                Amount = entity.Amount,
                IsActive = entity.IsActive
            };
        }

        public async Task<bool> DeleteAsync(int extraChargeId)
        {
            var entity = await _context.ExtraCharges.FirstOrDefaultAsync(x => x.ExtraChargeId == extraChargeId);
            if (entity == null) return false;
            entity.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 