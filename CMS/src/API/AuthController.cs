using CMS.src.Application.DTOs;
using CMS.src.Application.DTOs.Auth;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

namespace CMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [HttpPost("admin/create-user")]
        public async Task<IActionResult> AdminCreateUser([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _authService.RegisterByAdminAsync(registerDto);

                if (result)
                {
                    return Ok(new { message = "Usuario creado exitosamente. Se ha enviado un correo con la clave temporal" });
                }

                return BadRequest(new { message = "No se pudo crear el usuario." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _authService.FindByEmailAsync(dto.Email);

            if (user == null)
                return Unauthorized("Credenciales inválidas");

            if (!user.IsActive)
                return Unauthorized("Cuenta no confirmada");

            var passwordOk = await _authService.CheckPasswordAsync(user, dto.Password);
            if (!passwordOk)
                return Unauthorized("Credenciales inválidas");

            try
            {
                var result = await _authService.LoginAsync(dto);
                if (!result.Success)
                    return Unauthorized(result.Message);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + " | " + ex.InnerException?.Message);
            }

        }

        [HttpGet("activate")]
        public async Task<IActionResult> Activate([FromQuery] string token)
        {
            var result = await _authService.ActivateAccountAsync(token);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("confirm-account")]
        public async Task<IActionResult> ConfirmAccount([FromQuery] string email, [FromQuery] string token)
        {
            var success = await _authService.ConfirmAccountAsync(email, token);
            if (success)
            {
                return Redirect("http://localhost:5173/login?activated=true");
            }
            return BadRequest("Token inválido o expirado");
        }
       
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { message = "API conectada correctamente 🚀" });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();
                return Ok(users); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la lista", error = ex.Message });
            }
        }

        //[Authorize]
        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordDto request)
        {
            // Llamamos a tu método ChangePasswordAsync
            var result = await _authService.ChangePasswordAsync(request.Email, request.CurrentPassword, request.NewPassword);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }
    }
}
