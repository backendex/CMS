using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.src.API.Controller
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly IContentService _contentService;

        public ContentController(IContentService contentService)
        {
            _contentService = contentService;
        }

        [AllowAnonymous]
        [HttpGet("getPosts")]
        public async Task<IActionResult> GetPosts(string TableName, Guid siteId)
        {
            try
            {
                var posts = await _contentService.GetPostAsync(TableName, siteId);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al obtener posts",
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        [AllowAnonymous] 
        [HttpGet("getByIdPost")]
        public async Task<IActionResult> GetPostById(string TableName, long id)
        {
            try
            {
                var post = await _contentService.GetPostBySiteIdAsync(TableName, id);

                if (post == null)
                    return NotFound(new { message = "El post no existe." });

                return Ok(post);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el post", error = ex.Message });
            }
        }

        [HttpPost("createPost")]
        public async Task<IActionResult> CreatePost([FromBody] BlogPost postDto, string TableName)
        {
            try
            {
                var id = await _contentService.CreatePostAsync(postDto, TableName);
                return Ok(new { message = "Post guardado con éxito", id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("updatePost")]
        public async Task<IActionResult> UpdatePost([FromQuery] string TableName, [FromQuery] long id, [FromBody] BlogPost blogDto)
        {

            blogDto.Id = id;

            try
            {
                await _contentService.UpdatePostAsync(blogDto, TableName);
                return Ok(new { message = "Blog actualizado con éxito" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpDelete("deletePost")]
        public async Task<IActionResult> DeletePost([FromQuery] string TableName, [FromQuery] long id)
        {
            if (string.IsNullOrEmpty(TableName))
                return BadRequest(new { message = "El nombre de la tabla es requerido." });

            try
            {
                await _contentService.DeletePostAsync(id, TableName);
                return Ok(new { message = "Post eliminado con éxito" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el post", error = ex.Message });
            }
        }

        [HttpGet("{siteName}/media")]
        public async Task<IActionResult> GetMedia(string siteName, [FromQuery] Guid siteId)
        {
            var results = await _contentService.GetMediaBySiteAsync(siteId, siteName);
            return Ok(results);
        }

        [HttpPost("{siteName}/createMedia")]
        public async Task<IActionResult> CreateMedia(string siteName, [FromBody] MediaContent media)
        {
            var createdMedia = await _contentService.SaveMediaAsync(media, siteName);
            return Ok(createdMedia);
        }

        [HttpGet("{siteName}/categories")]
        public async Task<IActionResult> GetCategories(string siteName, [FromQuery] Guid siteId)
        {
            var categories = await _contentService.GetCategoriesAsync(siteId, siteName);
            return Ok(categories);
        }

        [HttpPost("{siteName}/createCategory")]
        public async Task<IActionResult> CreateCategory(string siteName, [FromBody] CategoryDto categoryDto)
        {
            var id = await _contentService.CreateCategoryAsync(categoryDto, siteName);
            return Ok(new { message = "Categoría creada", id });
        }
    }

}
    

