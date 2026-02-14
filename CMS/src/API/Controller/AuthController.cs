using CMS.src.Application.DTOs;
using CMS.src.Application.DTOs.Auth;
using CMS.src.Application.Interfaces;
using CMS.src.Application.Services;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static CMS.src.Application.DTOs.Auth.UserMethods;

namespace CMS.src.API.Controller
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [Authorize(Roles = "Administrador")]
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

        [Authorize(Roles = "Administrador")]
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

        [AllowAnonymous]
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

        [Authorize]
        [HttpPost("changePass")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Token inválido");

            var result = await _authService.ChangePasswordAsync(
                   int.Parse(userIdClaim),
                   dto.NewPassword
            );

            return result.Success ? Ok(result) : BadRequest(result);
        }


    }

}
