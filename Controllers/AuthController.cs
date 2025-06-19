using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TourManagement_BE.Data.DTO.Request;
using TourManagement_BE.Data.DTO.Response;
using TourManagement_BE.Service;

namespace TourManagement_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            await _authService.RegisterAsync(request);
            return Ok("Registration successful");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            await _authService.ForgotPasswordAsync(request);
            return Ok("Password reset email sent");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
    }

