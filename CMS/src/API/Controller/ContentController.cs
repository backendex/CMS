using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using CMS.src.Application.Services;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("site/{siteId}/text")]
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

        [HttpGet("site/{siteId}/media")]
        public async Task<IActionResult> GetBySite(Guid siteId)
        {
            var results = await _contentService.GetMediaBySiteAsync(siteId);
            return Ok(results);
        }

        [HttpPost("createMedia")]
        public async Task<IActionResult> Create([FromBody] MediaContent media)
        {
            var createdMedia = await _contentService.SaveMediaAsync(media);
            return Ok(createdMedia);
        }

        [HttpPost("createPost")]
        public async Task<IActionResult> CreatePost([FromBody] BlogPost postDto)
        {
            if (postDto == null) return BadRequest(new { message = "Los datos del post son requeridos." });

            try
            {
                var id = await _contentService.CreatePostAsync(postDto);
                return Ok(new { message = "Post guardado con éxito", id });
            }
            catch (Exception ex)
            {
                var errorDetail = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new { message = errorDetail });
            }
        }

        [HttpGet("getPosts")]
        public async Task<IActionResult> GetPosts(Guid siteId)
        {
            try
            {
                var posts = await _contentService.GetPostsAsync(siteId);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la lista de posts", error = ex.Message });
            }
        }

        [HttpGet("getPostById")]
        public async Task<IActionResult> GetPostById(Guid id, Guid siteId)
        {
            try
            {
                var post = await _contentService.GetPostByIdAsync(id, siteId);
                if (post == null || post.SiteId != siteId)
                {
                    return NotFound(new { message = "El blog solicitado no existe." });
                }
                return Ok(post);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el blog", error = ex.Message });
            }
        }

        [HttpPut("updatePost")]
        public async Task<IActionResult> UpdatePost(Guid id, [FromBody] BlogPost blogDto)
        {
            if (id != blogDto.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del cuerpo de la solicitud.");
            }
            try
            {
                await _contentService.UpdatePostAsync(blogDto);
                return Ok(new { message = "Blog actualizado con éxito" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("getCategories")]
        public async Task<IActionResult> GetCategories([FromQuery] Guid siteId)
        {
            try
            {
                var categories = await _contentService.GetCategoriesAsync(siteId);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener categorías", error = ex.Message });
            }
        }
        [HttpPost("createCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var id = await _contentService.CreateCategoryAsync(categoryDto);
                return Ok(new { message = "Categoría creada con éxito", id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
            
}
    

