using TourManagement_BE.Data.Models;
using System;
using System.Threading.Tasks;

namespace TourManagement_BE.Repository.Interface
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(Guid userId);
        Task AddUserAsync(User user);
        Task<Role> GetRoleByNameAsync(string roleName);
    }
}

