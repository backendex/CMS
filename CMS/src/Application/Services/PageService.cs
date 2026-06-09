using CMS.Infrastructure.Persistence;
using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.src.Application.Services
{
    public class PageService : IPageService
    {
        private readonly ApplicationDbContext _context;

        public PageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Page>> GetPagesBySiteAsync(Guid siteId)
        {
            return await _context.Pages
                .Include(p => p.Translations)
                .Where(p => p.SiteId == siteId)
                .ToListAsync();
        }

        public async Task<Page?> GetPageByIdAsync(Guid id)
        {
            return await _context.Pages
                .Include(p => p.Translations)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Page> CreatePageAsync(Guid siteId, string slug, string defaultTitle)
        {
            // Validar que el slug no exista en este sitio
            bool exists = await _context.Pages.AnyAsync(p => p.SiteId == siteId && p.Slug.ToLower() == slug.ToLower());
            if (exists)
            {
                throw new Exception("Ya existe una página con este slug en este sitio");
            }

            var page = new Page(siteId, slug);

            // Crear traducciones por defecto
            var esTranslation = new PageTranslation(page.Id, "es", defaultTitle, "[]");
            var enTranslation = new PageTranslation(page.Id, "en", defaultTitle, "[]");

            page.Translations.Add(esTranslation);
            page.Translations.Add(enTranslation);

            _context.Pages.Add(page);
            await _context.SaveChangesAsync();

            return page;
        }

        public async Task<Page> UpdatePageAsync(Guid id, PageSaveDto dto)
        {
            var page = await _context.Pages
                .Include(p => p.Translations)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (page == null)
            {
                throw new Exception("Página no encontrada");
            }

            // Validar que el nuevo slug no cause conflicto
            if (page.Slug.ToLower() != dto.Slug.ToLower())
            {
                bool exists = await _context.Pages.AnyAsync(p => p.SiteId == page.SiteId && p.Id != id && p.Slug.ToLower() == dto.Slug.ToLower());
                if (exists)
                {
                    throw new Exception("Ya existe otra página con este slug en este sitio");
                }
            }

            page.Slug = dto.Slug;
            page.IsPublished = dto.IsPublished;

            // Actualizar traducciones
            foreach (var transDto in dto.Translations)
            {
                var existingTrans = page.Translations.FirstOrDefault(t => t.Language.ToLower() == transDto.Language.ToLower());
                if (existingTrans != null)
                {
                    existingTrans.Title = transDto.Title;
                    existingTrans.BlocksJson = transDto.BlocksJson;
                }
                else
                {
                    var newTrans = new PageTranslation(page.Id, transDto.Language.ToLower(), transDto.Title, transDto.BlocksJson);
                    page.Translations.Add(newTrans);
                }
            }

            await _context.SaveChangesAsync();
            return page;
        }

        public async Task<Page> DeletePageAsync(Guid id)
        {
            var page = await _context.Pages
                .Include(p => p.Translations)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (page == null)
            {
                throw new Exception("Página no encontrada");
            }

            _context.Pages.Remove(page);
            await _context.SaveChangesAsync();

            return page;
        }

        public async Task<Page?> GetPageByDomainAndSlugAsync(string domain, string slug)
        {
            var site = await _context.Sites.FirstOrDefaultAsync(s => s.Domain.ToLower() == domain.ToLower());
            if (site == null)
            {
                return null;
            }

            return await _context.Pages
                .Include(p => p.Translations)
                .FirstOrDefaultAsync(p => p.SiteId == site.Id && p.Slug.ToLower() == slug.ToLower() && p.IsPublished);
        }

        public async Task<IEnumerable<Page>> GetPublicPagesByDomainAsync(string domain)
        {
            var site = await _context.Sites.FirstOrDefaultAsync(s => s.Domain.ToLower() == domain.ToLower());
            if (site == null)
            {
                return Enumerable.Empty<Page>();
            }

            return await _context.Pages
                .Include(p => p.Translations)
                .Where(p => p.SiteId == site.Id && p.IsPublished)
                .ToListAsync();
        }
    }
}
