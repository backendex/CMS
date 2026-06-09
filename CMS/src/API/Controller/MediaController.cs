using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.src.API.Controller
{
    [ApiController]
    [Route("api/media")]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;

        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [HttpGet("auth/imagekit")]
        public IActionResult GetAuth() => Ok(_mediaService.GetImageKitAuth());

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] SaveMediaDto dto) =>
            Ok(await _mediaService.SaveMediaAsync(dto));

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string siteId) =>
            Ok(await _mediaService.GetMediaBySiteAsync(siteId));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediaService.DeleteMediaAsync(id);
            return NoContent();
        }
    }

}
