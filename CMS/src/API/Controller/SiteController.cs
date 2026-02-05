using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace CMS.src.API.Controller
{
    [Authorize]
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

        [Authorize]
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

    }
}
