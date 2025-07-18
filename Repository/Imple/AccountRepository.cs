using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.DTO.Response.AccountResponse;
using TourManagement_BE.Models;
using TourManagement_BE.Repository.Interface;

namespace TourManagement_BE.Repository.Imple
{
    public class AccountRepository : IAccountRepository
    {
        private readonly MyDBContext _context;

        public AccountRepository(MyDBContext context)
        {
            _context = context;
        }
        public async Task<List<ListAccountResponse>> ListAllAccount()
        {
            var users = await _context.Users.Select(u => new ListAccountResponse
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                Address = u.Address,
                PhoneNumber = u.PhoneNumber,
                Avatar = u.Avatar,
                RoleName = u.Role.RoleName,
                IsActive = u.IsActive
            }).ToListAsync();

            return users; 
        }
    }
}
