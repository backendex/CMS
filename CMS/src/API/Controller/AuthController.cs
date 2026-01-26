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

namespace CMS.src.API.Controller
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
        //Aqui una llave de admin
        //[Authorize(Roles = "Admin")]
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
            try
            {
                var result = await _authService.LoginAsync(dto);

                if (!result.Success)
                {
                    return Unauthorized(new { message = result.Message });
                }

                return Ok(new
                {
                    token = result.Token,
                    mustChangePassword = result.MustChangePassword
                });
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

        //Nuevo controlador para pedir token
        [HttpGet("confirm-account")]
        public async Task<IActionResult> ConfirmAccount([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token inválido");

            var success = await _authService.ConfirmAccountAsync(token);

            if (!success)
                return BadRequest("El enlace es inválido o expiró.");

            return Redirect("http://localhost:5173/login?activated=true");
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { message = "API conectada correctamente" });
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
        [HttpGet("debug-config")]
        public IActionResult DebugConfig(IConfiguration config)
        {
            return Ok(new
            {
                Env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                ApiKey = config["Resend:ApiKey"],
                From = config["Resend:FromEmail"]
            });
        }

    }
}
