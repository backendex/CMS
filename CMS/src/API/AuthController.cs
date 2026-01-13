using CMS.src.Application.DTOs;
using CMS.src.Application.DTOs.Auth;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
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
        [HttpPost("admin/create-user")]
        public async Task<IActionResult> AdminCreateUser([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Ahora el servicio devuelve un bool o lanza una excepción si algo falla
                var result = await _authService.RegisterByAdminAsync(registerDto);

                if (result)
                {
                    return Ok(new { message = "Usuario creado exitosamente. Se ha enviado un correo con la clave temporal" });
                }

                return BadRequest(new { message = "No se pudo crear el usuario." });
            }
            catch (Exception ex)
            {
                // Capturamos errores como "El rol no existe" o "El email ya está registrado"
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _authService.FindByEmailAsync(dto.Email);

            if (user == null)
                return Unauthorized("Credenciales inválidas");

            if (!user.EmailConfirmed)
                return Unauthorized("Cuenta no confirmada");

            var passwordOk = await _authService.CheckPasswordAsync(user, dto.Password);
            if (!passwordOk)
                return Unauthorized("Credenciales inválidas");


            return Ok(new
            {
                
            });
        }

        [HttpGet("activate")]
        public async Task<IActionResult> Activate([FromQuery] string token)
        {
            var result = await _authService.ActivateAccountAsync(token);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }

        //Para realizar pruebas de api
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { message = "API conectada correctamente 🚀" });
        }

        [HttpGet("confirm-account")]
        public async Task<IActionResult> ConfirmAccount([FromQuery] string email, [FromQuery] string token)
        {
            var success = await _authService.ConfirmAccountAsync(email, token);
            return success ? Ok("Cuenta activada") : BadRequest("Token inválido");
        }


    }
}
