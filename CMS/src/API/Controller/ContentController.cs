using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.src.API.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly IContentService _contentService;

        public ContentController(IContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpGet("site/{siteId}")]
        public async Task<IActionResult> GetContent(Guid siteId)
        {
            var content = await _contentService.GetContentBySiteIdAsync(siteId);
            return Ok(content);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> updateBulk([FromBody] List<ContentUpdateDto> contentRequests)
        {
            
            if (contentRequests == null || !contentRequests.Any())
            {
                return BadRequest("La lista de cambios está vacía.");
            }
            var success = await _contentService.UpdateBulkContentAsync(contentRequests);

            if (!success)
            {
                return BadRequest("No se realizaron cambios o hubo un error al actualizar.");
            }
            return Ok(new { message = "Contenido sincronizado correctamente" });
        }
    }
}
