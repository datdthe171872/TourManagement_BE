using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.AccountRequest;
using TourManagement_BE.Data.DTO.Response.AccountResponse;

namespace TourManagement_BE.Service.AccountManagement
{
    // Services/AccountService/AccountService.cs
    public class AccountService : IAccountService
    {
        private readonly MyDBContext _context;

        public AccountService(MyDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ListAccountResponse>> GetAllAccountsAsync()
        {
            return await _context.Users
                .Select(u => new ListAccountResponse
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    Address = u.Address,
                    PhoneNumber = u.PhoneNumber,
                    Avatar = u.Avatar,
                    RoleName = u.Role.RoleName,
                    IsActive = u.IsActive
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ListAccountResponse>> SearchAccountsAsync(string keyword)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(keyword) ||
                    u.Email.ToLower().Contains(keyword) ||
                    u.UserId.ToString().Contains(keyword)
                );
            }

            return await query
                .Select(u => new ListAccountResponse
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    Address = u.Address,
                    PhoneNumber = u.PhoneNumber,
                    Avatar = u.Avatar,
                    RoleName = u.Role.RoleName,
                    IsActive = u.IsActive
                })
                .ToListAsync();
        }

        public async Task<PagedResult<ListAccountResponse>> GetAllAccountsPagedAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Users.AsQueryable();

            var totalRecords = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.UserId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new ListAccountResponse
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    Address = u.Address,
                    PhoneNumber = u.PhoneNumber,
                    Avatar = u.Avatar,
                    RoleName = u.Role.RoleName,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return new PagedResult<ListAccountResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = users
            };
        }

        public async Task<PagedResult<ListAccountResponse>> SearchAccountsPagedAsync(string keyword, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(keyword) ||
                    u.Email.ToLower().Contains(keyword) ||
                    u.UserId.ToString().Contains(keyword)
                );
            }

            var totalRecords = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.UserId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new ListAccountResponse
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    Address = u.Address,
                    PhoneNumber = u.PhoneNumber,
                    Avatar = u.Avatar,
                    RoleName = u.Role.RoleName,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return new PagedResult<ListAccountResponse>
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = users
            };
        }

        public async Task<UpdateResult> UpdateAccountStatusAsync(UpdateStatusRequest request)
        {
            var result = new UpdateResult();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
            if (user == null)
            {
                result.Success = false;
                result.Message = "User not found.";
                return result;
            }

            user.IsActive = request.IsActive;
            await _context.SaveChangesAsync();

            result.Success = true;
            result.Message = "User status updated successfully.";
            result.Account = new ListAccountResponse
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                RoleName = user.Role.RoleName,
                IsActive = user.IsActive
            };

            return result;
        }

        public async Task<UpdateResult> ToggleAccountStatusAsync(int userId)
        {
            var result = new UpdateResult();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                result.Success = false;
                result.Message = "User not found.";
                return result;
            }

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            result.Success = true;
            result.Message = "User status updated successfully.";
            result.Account = new ListAccountResponse
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                RoleName = user.Role.RoleName,
                IsActive = user.IsActive
            };

            return result;
        }
    }
}
