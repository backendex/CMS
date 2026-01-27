using CMS.src.Application.Interfaces;
using CMS.src.Infrastructure.Persistence.Repositories;

namespace CMS.src.Middlewares
{
    public class SiteResolutionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISiteService _siteService;

        public SiteResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
            ISiteService _siteService;

        }

        public async Task InvokeAsync(
            HttpContext context,
            ISiteService _siteService
        )
        {
            var host = context.Request.Host.Host;

            var normalizedDomain = NormalizeDomain(host);

            var site = await _siteService.GetByDomainAsync(normalizedDomain);

            if (site == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Site not found");
                return;
            }

            context.Items["Site"] = site;

            await _next(context);
        }

        private static string NormalizeDomain(string host)
        {
            // Quita prefijos técnicos
            if (host.StartsWith("cms."))
                host = host.Replace("cms.", "");

            if (host.StartsWith("www."))
                host = host.Replace("www.", "");

            return host.ToLowerInvariant();
        }
    }

}
