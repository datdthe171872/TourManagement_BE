using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data.Context;
using TourManagement_BE.Data.DTO.Request.AccountRequest;
using TourManagement_BE.Data.DTO.Response.AccountResponse;
using TourManagement_BE.Repository.Interface;
using TourManagement_BE.Service.AccountManagement;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly MyDBContext context;
        private readonly IAccountRepository _account;
        private readonly IAccountService _accountService;

        public AccountController(MyDBContext context, IAccountRepository account, IAccountService accountService)
        {
            this.context = context;
            _account = account;
            _accountService = accountService;
        }

        [HttpGet("View All Account")]
        public async Task<IActionResult> ListAllAccount()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            if (accounts == null || !accounts.Any())
            {
                return NotFound("Not Found.");
            }
            return Ok(accounts);
        }

        [HttpGet("Search Account By Name or ID or Email")]
        public async Task<IActionResult> SearchAccount(string? keyword)
        {
            var accounts = await _accountService.SearchAccountsAsync(keyword);
            if (accounts == null || !accounts.Any())
            {
                return NotFound("No users found.");
            }
            return Ok(accounts);
        }

        [HttpGet("PagingAllAccount")]
        public async Task<IActionResult> PagingAllAccount(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _accountService.GetAllAccountsPagedAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("PagingSearchAccount")]
        public async Task<IActionResult> PagingSearchAccount(string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _accountService.SearchAccountsPagedAsync(keyword, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPut("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            var result = await _accountService.UpdateAccountStatusAsync(request);
            if (!result.Success)
            {
                return NotFound(result.Message);
            }
            return Ok(new { message = result.Message, account = result.Account });
        }

        [HttpPut("ToggleStatus/{userId}")]
        public async Task<IActionResult> ToggleStatus(int userId)
        {
            var result = await _accountService.ToggleAccountStatusAsync(userId);
            if (!result.Success)
            {
                return NotFound(result.Message);
            }
            return Ok(new { message = result.Message, account = result.Account });
        }

    }
}
