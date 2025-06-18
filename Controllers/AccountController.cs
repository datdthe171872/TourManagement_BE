using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.AccountRequest;
using TourManagement_BE.Data.DTO.Response.AccountResponse;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly MyDBContext context;

        public AccountController(MyDBContext context)
        {
            this.context = context;
        }

        [HttpGet("View All Account")]
        public IActionResult ListAllAccount()
        {
            var user = context.Users.Select(u => new ListAccountResponse
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                Address = u.Address,
                PhoneNumber = u.PhoneNumber,
                Avatar = u.Avatar,
                RoleName = u.Role.RoleName,
                IsActive = u.IsActive
            });

            if (user == null)
            {
                return NotFound("Not Found.");
            }

            return Ok(user);
        }

        [HttpGet("Search Account By Name or ID or Email")]
        public IActionResult SearchAccount(string? keyword)
        {
            var query = context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(keyword) ||
                    u.Email.ToLower().Contains(keyword) ||
                    u.UserId.ToString().Contains(keyword)
                );
            }

            var users = query.Select(u => new ListAccountResponse
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                Address = u.Address,
                PhoneNumber = u.PhoneNumber,
                Avatar = u.Avatar,
                RoleName = u.Role.RoleName,
                IsActive = u.IsActive
            }).ToList();

            if (!users.Any())
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }

        [HttpGet("PagingAllAccount")]
        public IActionResult PagingAllAccount(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalRecords = context.Users.Count();

            var users = context.Users
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
                .ToList();

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = users
            });
        }

        [HttpGet("PagingSearchAccount")]
        public IActionResult PagingSearchAccount(string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u =>
                    u.UserName.ToLower().Contains(keyword) ||
                    u.Email.ToLower().Contains(keyword) ||
                    u.UserId.ToString().Contains(keyword)
                );
            }

            var totalRecords = query.Count();

            var users = query
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
                .ToList();

            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Data = users
            });
        }

        [HttpPut("UpdateStatus")]
        public IActionResult UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            var user = context.Users.FirstOrDefault(u => u.UserId == request.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.IsActive = request.IsActive;
            context.SaveChanges();

            return Ok(new { message = "User status updated successfully." });
        }

    }
}
