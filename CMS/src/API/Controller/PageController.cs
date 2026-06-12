using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.src.API.Controller
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/page")]
    public class PageController : ControllerBase
    {
        private readonly IPageService _pageService;

        public PageController(IPageService pageService)
        {
            _pageService = pageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPagesBySite([FromQuery] Guid siteId)
        {
            try
            {
                var pages = await _pageService.GetPagesBySiteAsync(siteId);
                return Ok(pages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener páginas", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPageById(Guid id)
        {
            try
            {
                var page = await _pageService.GetPageByIdAsync(id);
                if (page == null)
                    return NotFound(new { message = "Página no encontrada" });
                return Ok(page);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la página", error = ex.Message });
            }
        }

        public class CreatePageRequest
        {
            public string Slug { get; set; } = null!;
            public string Title { get; set; } = null!;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePage([FromQuery] Guid siteId, [FromBody] CreatePageRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Slug))
                {
                    return BadRequest(new { message = "El slug es requerido" });
                }

                var createdPage = await _pageService.CreatePageAsync(siteId, request.Slug, request.Title ?? request.Slug);
                return CreatedAtAction(nameof(GetPageById), new { id = createdPage.Id }, createdPage);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al crear la página", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePage(Guid id, [FromBody] PageSaveDto dto)
        {
            try
            {
                var updatedPage = await _pageService.UpdatePageAsync(id, dto);
                return Ok(updatedPage);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al actualizar la página", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePage(Guid id)
        {
            try
            {
                await _pageService.DeletePageAsync(id);
                return Ok(new { message = "Página eliminada con éxito" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al eliminar la página", error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("debug-db")]
        public async Task<IActionResult> DebugDb([FromServices] IApplicationDbContext db)
        {
            try
            {
                var sites = await db.Sites.ToListAsync();
                var pages = await db.Pages.ToListAsync();
                var pageTranslations = await db.PageTranslations.ToListAsync();
                return Ok(new { sites, pages, pageTranslations });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error debugging db", error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("public")]
        public async Task<IActionResult> GetPagePublic([FromQuery] string domain, [FromQuery] string slug)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(domain) || string.IsNullOrWhiteSpace(slug))
                {
                    return BadRequest(new { message = "El dominio y el slug son requeridos" });
                }

                var page = await _pageService.GetPageByDomainAndSlugAsync(domain, slug);
                if (page == null)
                {
                    return NotFound(new { message = "Página no encontrada o no publicada" });
                }

                return Ok(page);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la página pública", error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("public/list")]
        public async Task<IActionResult> GetPublicPagesList([FromQuery] string domain)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(domain))
                {
                    return BadRequest(new { message = "El dominio es requerido" });
                }

                var pages = await _pageService.GetPublicPagesByDomainAsync(domain);
                return Ok(pages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la lista de páginas públicas", error = ex.Message });
            }
        }
    }
}
