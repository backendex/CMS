using CMS.src.Application.DTOs.Post;
using CMS.src.Application.Interfaces;
using CMS.src.Application.Services;
using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CMS.src.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostDto dto)
        {
            var authorId = int.Parse(User.FindFirst("id")!.Value);

            await _postService.CreateAsync(dto, authorId);
            return Ok();
        }

        [HttpPost("admin/publish")]
        public async Task<IActionResult> Publish(Guid id)
        {
            await _postService.PublishAsync(id);
            return NoContent();
        }
    }

}
