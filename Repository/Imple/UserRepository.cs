using Microsoft.EntityFrameworkCore;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Repository.Interface;
using System.Threading.Tasks;

namespace TourManagement_BE.Repository.Imple
{
    public class UserRepository : IUserRepository
    {
        private readonly MyDBContext _context;
        public UserRepository(MyDBContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);
        }

        public async Task<User> GetUserByIdForVerificationAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == roleName && r.IsActive);
        }

        public async Task AddResetPasswordTokenAsync(ResetPasswordToken token)
        {
            await _context.ResetPasswordTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<ResetPasswordToken> GetResetPasswordTokenAsync(string token)
        {
            return await _context.ResetPasswordTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed && t.ExpiryDate > System.DateTime.UtcNow);
        }

        public async Task SetResetPasswordTokenUsedAsync(int tokenId)
        {
            var token = await _context.ResetPasswordTokens.FindAsync(tokenId);
            if (token != null)
            {
                token.IsUsed = true;
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
