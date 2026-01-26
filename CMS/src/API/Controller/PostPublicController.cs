using CMS.src.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMS.src.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsPublicController : ControllerBase
    {
        private readonly IPostRepository _postRepository;

        public PostsPublicController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] Guid siteId,
            [FromQuery] string lang = "es")

    {
            var posts = await _postRepository.GetPublishedBySiteAsync(siteId);

            var result = posts.Select(p =>
                p.Translations.First(t => t.Language == lang)
            );

            return Ok(result);
        }
    }

}
