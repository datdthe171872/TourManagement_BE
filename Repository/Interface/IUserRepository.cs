using TourManagement_BE.Data;
using System;
using System.Threading.Tasks;
using TourManagement_BE.Models;

namespace TourManagement_BE.Repository.Interface
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int userId);
        Task AddUserAsync(User user);
        Task<Role> GetRoleByNameAsync(string roleName);
        Task AddResetPasswordTokenAsync(ResetPasswordToken token);
        Task<ResetPasswordToken> GetResetPasswordTokenAsync(string token);
        Task SetResetPasswordTokenUsedAsync(int tokenId);
        Task UpdateUserAsync(User user);
    }
}

