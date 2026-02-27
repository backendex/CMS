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

        // --- BLOG POSTS ---

        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts(string siteName,[FromQuery] Guid siteId)
        {
            try
            {
                var posts = await _contentService.GetPostAsync(siteName,siteId);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener posts", error = ex.Message });
            }
        }

        [HttpGet("{siteName}/posts/{id}")]
        public async Task<IActionResult> GetPostById(string siteName, long id, Guid siteId)
        {
            var post = await _contentService.GetPostByIdAsync(siteName, id, siteId);
            if (post == null) return NotFound(new { message = "El post no existe." });
            return Ok(post);
        }

        [HttpPost("createPost")]
        public async Task<IActionResult> CreatePost([FromBody] BlogPost postDto, string siteName)
        {
            try
            {
                var id = await _contentService.CreatePostAsync(postDto, siteName);
                return Ok(new { message = "Post guardado con éxito", id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{siteName}/updatePost/{id}")]
        public async Task<IActionResult> UpdatePost(string siteName, long id, [FromBody] BlogPost blogDto)
        {
            if (id != blogDto.Id) return BadRequest("ID no coincide.");

            try
            {
                await _contentService.UpdatePostAsync(blogDto, siteName);
                return Ok(new { message = "Blog actualizado con éxito" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // --- MEDIA ---

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

        // --- CATEGORIES ---

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
    

