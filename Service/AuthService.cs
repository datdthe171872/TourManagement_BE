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
using System.Linq;
using Google.Apis.Auth;

namespace TourManagement_BE.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _configuration;
        private readonly MyDBContext _context;
        private readonly INotificationService _notificationService;
        private readonly EmailHelper _emailHelper;

        public AuthService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration, MyDBContext context, INotificationService notificationService, JwtHelper jwtHelper, EmailHelper emailHelper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _context = context;
            _notificationService = notificationService;
            _jwtHelper = jwtHelper;
            _emailHelper = emailHelper;
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
            // Check for any existing users with the same email (both active and inactive)
            var existingUsers = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Email == request.Email)
                .ToListAsync();
            
            // Special handling for Tour Guide registration
            if (request.RoleName == Roles.TourGuide)
            {
                // Check if there's any active user with the same email
                var activeUser = existingUsers.FirstOrDefault(u => u.IsActive);
                if (activeUser != null)
                {
                    throw new Exception("Email already exists and is active");
                }
                
               
            }
            else
            {
                // For other roles, check if email exists regardless of status
                if (existingUsers.Any())
                {
                    throw new Exception("Email already exists");
                }
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

            // Tạo notification khi đăng ký thành công
            await _notificationService.CreateRegistrationSuccessNotificationAsync(createdUser.UserId);

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

                // Send email to tour guide with password
                try
                {
                    var subject = "Tour Guide Account Created - Tour Management System";
                    var body = $@"
                        <html>
                        <body>
                            <h2>Tour Guide Account Created Successfully</h2>
                            <p>Hello {request.UserName},</p>
                            <p>Your tour guide account has been created successfully by the tour operator.</p>
                            <p><strong>Login Credentials:</strong></p>
                            <ul>
                                <li><strong>Email:</strong> {request.Email}</li>
                                <li><strong>Password:</strong> {request.Password}</li>
                            </ul>
                            <p>Please use these credentials to log in to your account.</p>
                            <p>For security reasons, we recommend changing your password after your first login.</p>
                            <p>Best regards,<br>Tour Management System Team</p>
                        </body>
                        </html>";

                    await _emailHelper.SendEmailAsync(request.Email, subject, body);
                }
                catch (Exception ex)
                {
                    // Log the email sending error but don't fail the registration
                    // You might want to add proper logging here
                }

                // Send notification to tour operator
                if (tourOperatorId.HasValue)
                {
                    var tourOperator = await _context.TourOperators
                        .Include(to => to.User)
                        .FirstOrDefaultAsync(to => to.TourOperatorId == tourOperatorId.Value);
                    
                    if (tourOperator?.User != null)
                    {
                        await _notificationService.CreateNotificationAsync(
                            tourOperator.User.UserId,
                            "Tour Guide Registration Success",
                            $"Tour guide {request.UserName} has been registered successfully.",
                            "TourGuideRegistration"
                        );
                    }
                }
            }
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception("Email not found");
            }

            // Kiểm tra user có active không
            if (!user.IsActive)
            {
                throw new Exception("User account is not active");
            }

            // Xóa các token cũ của user này
            var existingTokens = await _context.ResetPasswordTokens
                .Where(t => t.UserId == user.UserId && !t.IsUsed)
                .ToListAsync();
            
            if (existingTokens.Any())
            {
                _context.ResetPasswordTokens.RemoveRange(existingTokens);
                await _context.SaveChangesAsync();
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
            var subject = "Password Reset Request - Tour Management System";
            var body = $@"
                <html>
                <body>
                    <h2>Password Reset Request</h2>
                    <p>Hello {user.UserName},</p>
                    <p>You have requested to reset your password for your Tour Management System account.</p>
                    <p>Click the button below to reset your password:</p>
                    <p>
                        <a href='{resetLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Reset Password
                        </a>
                    </p>
                    <p>Or copy and paste this link into your browser:</p>
                    <p>{resetLink}</p>
                    <p><strong>This link will expire in 1 hour.</strong></p>
                    <p>If you didn't request this password reset, please ignore this email.</p>
                    <p>Best regards,<br>Tour Management System Team</p>
                </body>
                </html>";

            try
            {
                var emailHelper = new EmailHelper(_configuration);
                await emailHelper.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception ex)
            {
                // Xóa token nếu gửi email thất bại
                _context.ResetPasswordTokens.Remove(tokenEntity);
                await _context.SaveChangesAsync();
                throw new Exception($"Failed to send reset email: {ex.Message}");
            }
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

            // Kiểm tra user có active không
            if (!user.IsActive)
            {
                throw new Exception("User account is not active");
            }

            // Kiểm tra password mới có khác password cũ không
            if (PasswordHelper.VerifyPassword(request.NewPassword, user.Password))
            {
                throw new Exception("New password must be different from the current password");
            }

            // Cập nhật password
            user.Password = PasswordHelper.HashPassword(request.NewPassword);
            
            // Đánh dấu token đã sử dụng
            await _userRepository.SetResetPasswordTokenUsedAsync(tokenEntity.Id);
            
            // Lưu thay đổi password
            await _userRepository.UpdateUserAsync(user);

            // Gửi email thông báo password đã được reset thành công
            try
            {
                var subject = "Password Reset Successful - Tour Management System";
                var body = $@"
                    <html>
                    <body>
                        <h2>Password Reset Successful</h2>
                        <p>Hello {user.UserName},</p>
                        <p>Your password has been successfully reset for your Tour Management System account.</p>
                        <p>If you did not perform this action, please contact our support team immediately.</p>
                        <p>Best regards,<br>Tour Management System Team</p>
                    </body>
                    </html>";

                var emailHelper = new EmailHelper(_configuration);
                await emailHelper.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception ex)
            {
                // Log lỗi gửi email nhưng không throw exception vì password đã được reset thành công
                // Có thể thêm logging service ở đây
                Console.WriteLine($"Failed to send password reset confirmation email: {ex.Message}");
            }
        }
        
        public async Task<LoginResponse> AuthWithGoogleAsync (string TokenId)
        {
            try
            {
                // Xác thực token Google
                var payload = await GoogleJsonWebSignature.ValidateAsync(TokenId);
                string email = payload.Email;
                string name = payload.Name;
                string picture = payload.Picture;

                // Kiểm tra user trong DB
                var user = _context.Users
                .Include(u => u.Role)
                .Where(u => u.Email == email)
                .FirstOrDefault(x => x.Email == email && x.IsActive ==true);
                var role = _context.Roles.FirstOrDefault(x => x.RoleName == Roles.Customer);
                if (user == null)
                {
                    // Nếu chưa có user, tự động đăng ký
                    user = new User {
                        Email = email, 
                        UserName = name, 
                        Avatar = picture,
                        RoleId = role.RoleId,
                        Password = "123456",
                        IsActive = true,
                    };
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();
                }

                // Sinh JWT
                var token = _jwtHelper.GenerateToken(user.UserId.ToString(), user.Email, user.Role.RoleName);

                return new LoginResponse
                {
                    Token = token,
                    UserId = user.UserId.ToString(),
                    Email = user.Email,
                    RoleName = user.Role.RoleName,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
