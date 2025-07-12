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
using TourManagement_BE.Data.Context;
using Microsoft.EntityFrameworkCore;
namespace TourManagement_BE.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _configuration;
        private readonly MyDBContext _context;

        public AuthService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration, MyDBContext context)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _context = context;
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

        public async Task RegisterAsync(RegisterRequest request, int? tourOperatorId = null)
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
            user.Password = PasswordHelper.HashPassword(request.Password);
            user.RoleId = role.RoleId;
            user.IsActive = true;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Lấy lại userId vừa tạo
            var createdUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

            if (request.RoleName == Roles.TourGuide)
            {
                var tourGuide = new TourGuide
                {
                    UserId = createdUser.UserId,
                    TourOperatorId = tourOperatorId,
                    IsActive = true
                };
                _context.TourGuides.Add(tourGuide);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception("Email not found");
            }

            var resetToken = Guid.NewGuid().ToString();
            var expiry = DateTime.UtcNow.AddHours(1);
            var tokenEntity = new ResetPasswordToken
            {
                UserId = user.UserId,
                Token = resetToken,
                ExpiryDate = expiry,
                IsUsed = false
            };
            await _userRepository.AddResetPasswordTokenAsync(tokenEntity);

            var resetLink = $"http://localhost:3000/reset-password?token={resetToken}";
            var subject = "Password Reset Request";
            var body = $"<p>Click the link below to reset your password:</p><p><a href='{resetLink}'>Reset Password</a></p>";
            var emailHelper = new EmailHelper(_configuration);
            await emailHelper.SendEmailAsync(user.Email, subject, body);
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            var tokenEntity = await _userRepository.GetResetPasswordTokenAsync(request.Token);
            if (tokenEntity == null)
            {
                throw new Exception("Invalid or expired token");
            }
            var user = await _userRepository.GetUserByIdAsync(tokenEntity.UserId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            user.Password = PasswordHelper.HashPassword(request.NewPassword);
            await _userRepository.SetResetPasswordTokenUsedAsync(tokenEntity.Id);
            await _userRepository.UpdateUserAsync(user); // Save password change
        }
    }
}
