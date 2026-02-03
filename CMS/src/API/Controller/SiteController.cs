using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace CMS.src.API.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SiteController : ControllerBase
    {
        private readonly ISiteService _siteService;
        private readonly IApplicationDbContext _context;

        public SiteController(ISiteService siteService, IApplicationDbContext context)
        {
            _siteService = siteService;
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("getUserAccess/{userId}")]
        public async Task<IActionResult> GetUserAccess(int userId)
        {
            var response = await _siteService.GetUserAccessAsync(userId);

            if (response == null)
                return NotFound(new { message = "Usuario no encontrado" });

            return Ok(response);
        }
    }
}
