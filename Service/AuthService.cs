using AutoMapper;
using Microsoft.Extensions.Configuration;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Data.Models;
using TourManagement_BE.Helper.Common;
using TourManagement_BE.Helper.Constant;
using TourManagement_BE.Repository.Interface;
using TourManagement_BE.Data.DTO.Request;
using System;
using System.Threading.Tasks;
namespace TourManagement_BE.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _jwtHelper = new JwtHelper(
                _configuration["Jwt:SecretKey"],
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"]);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.Password))
            {
                throw new Exception("Invalid email or password");
            }

            var token = _jwtHelper.GenerateToken(user.UserId.ToString(), user.Email, user.Role.RoleName);
            return new LoginResponse
            {
                Token = token,
                UserId = user.UserId.ToString(),
                Email = user.Email,
                RoleName = user.Role.RoleName
            };
        }

        public async Task RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new Exception("Email already exists");
            }

            var role = await _userRepository.GetRoleByNameAsync(request.RoleName);
            if (role == null || !Roles.AllRoles.Contains(request.RoleName))
            {
                throw new Exception("Invalid role. Must be one of: " + string.Join(", ", Roles.AllRoles));
            }

            var user = _mapper.Map<User>(request);
            user.UserId = user.UserId;
            user.Password = PasswordHelper.HashPassword(request.Password);
            user.RoleId = role.RoleId;
            user.IsActive = true;
            await _userRepository.AddUserAsync(user);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception("Email not found");
            }

            // Giả lập gửi email với link reset password
            var resetToken = Guid.NewGuid().ToString();
            // TODO: Lưu resetToken vào database hoặc cache với thời gian hết hạn
            // Gửi email chứa link: https://your-app.com/reset-password?token={resetToken}
            // Ví dụ: await _emailService.SendResetPasswordEmail(user.Email, resetToken);
            throw new NotImplementedException("Email service not implemented");
        }
    }
}
