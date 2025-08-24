using TourManagement_BE.Data.Models;
using System;
using System.Threading.Tasks;

namespace TourManagement_BE.Repository.Interface
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserByIdForVerificationAsync(int userId);
        Task AddUserAsync(User user);
        Task<Role> GetRoleByNameAsync(string roleName);
        Task AddResetPasswordTokenAsync(ResetPasswordToken token);
        Task<ResetPasswordToken> GetResetPasswordTokenAsync(string token);
        Task SetResetPasswordTokenUsedAsync(int tokenId);
        Task UpdateUserAsync(User user);
    }
}

