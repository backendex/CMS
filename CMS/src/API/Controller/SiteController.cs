using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.src.API.Controller
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/site")]
    public class SiteController : ControllerBase
    {
        private readonly ISiteService _siteService;
        private readonly IApplicationDbContext _context;

        public SiteController(ISiteService siteService, IApplicationDbContext context)
        {
            _siteService = siteService;
            _context = context;
        }

        [HttpGet("user-access")]
        public async Task<IActionResult> GetUserAccess()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var response = await _siteService.GetUserAccessAsync(userId);

            if (response == null)
                return NotFound(new { message = "Usuario no encontrado" });

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var sites = await _siteService.GetAllSitesAsync();
                return Ok(sites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener sitios", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var site = await _siteService.GetByIdAsync(id);
                if (site == null)
                    return NotFound(new { message = "Sitio no encontrado" });
                return Ok(site);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener sitio", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SiteDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            try
            {
                var createdSite = await _siteService.CreateSiteAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = createdSite.Id }, createdSite);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al crear sitio", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SiteDto dto)
        {
            try
            {
                var updatedSite = await _siteService.UpdateSiteAsync(id, dto);
                return Ok(updatedSite);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al actualizar sitio", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _siteService.DeleteSiteAsync(id);
                return Ok(new { message = "Sitio eliminado con éxito" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al eliminar sitio", error = ex.Message });
            }
        }
    }
}
