using CMS.src.Application.DTOs;
using CMS.src.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Esto define la URL: api/auth
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }   

        [HttpPost("register")] // URL: api/auth/register
        public async Task<IActionResult> Register(UserRegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpPost("login")] // URL: api/auth/login
        public async Task<IActionResult> Login(LoginRequest loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (!result.IsSuccess) return Unauthorized(result.Message);
            return Ok(result);
        }
    }
}