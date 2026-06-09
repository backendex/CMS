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
    public class SiteService : ISiteService
    {
        private readonly ApplicationDbContext _context;

        public SiteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Site> GetByDomainAsync(string domain)
        {
            var site = await _context.Sites.FirstOrDefaultAsync(s => s.Domain.ToLower() == domain.ToLower());
            if (site == null)
                throw new Exception($"Sitio con dominio '{domain}' no encontrado");
            return site;
        }

        public async Task<IEnumerable<Site>> GetAllSitesAsync()
        {
            return await _context.Sites.OrderBy(s => s.Name).ToListAsync();
        }

        public async Task<Site?> GetByIdAsync(Guid id)
        {
            return await _context.Sites.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Site> CreateSiteAsync(SiteDto dto, int userId)
        {
            string cleanName = dto.Domain.ToLower().Replace(".", "_").Replace("-", "_");
            string tableName = $"wp_{cleanName}";

            // 1. Crear la tabla de posts dinámica si no existe
            string createTableSql = $@"
                CREATE TABLE IF NOT EXISTS ""{tableName}"" (
                    id BIGSERIAL PRIMARY KEY,
                    post_author INTEGER NULL,
                    post_date VARCHAR(30) NULL,
                    post_date_gmt VARCHAR(30) NULL,
                    post_content TEXT NULL,
                    post_title TEXT NULL,
                    post_excerpt TEXT NULL,
                    post_status VARCHAR(20) NULL,
                    comment_status VARCHAR(20) NULL,
                    ping_status VARCHAR(20) NULL,
                    post_password VARCHAR(255) NULL,
                    post_name VARCHAR(200) NULL,
                    to_ping TEXT NULL,
                    pinged TEXT NULL,
                    post_modified VARCHAR(30) NULL,
                    post_modified_gmt VARCHAR(30) NULL,
                    post_content_filtered TEXT NULL,
                    post_parent BIGINT NULL,
                    guid VARCHAR(255) NULL,
                    menu_order INTEGER NULL,
                    post_type VARCHAR(20) NULL,
                    post_mime_type VARCHAR(100) NULL,
                    comment_count BIGINT NULL,
                    site_id UUID NOT NULL,
                    seo_data JSONB NULL
                );";
            
            await _context.Database.ExecuteSqlRawAsync(createTableSql);

            // 2. Crear y guardar el nuevo sitio
            var site = new Site
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Domain = dto.Domain,
                Url = dto.Url ?? $"https://{dto.Domain}",
                Color = dto.Color ?? "#000000",
                IsMaintenance = dto.IsMaintenance,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                TableName = tableName
            };

            _context.Sites.Add(site);
            await _context.SaveChangesAsync();

            // 3. Crear relación UserSite para dar acceso al usuario creador
            var userSite = new UserSite
            {
                UserId = userId,
                SiteId = site.Id
            };
            _context.Set<UserSite>().Add(userSite);
            await _context.SaveChangesAsync();

            return site;
        }

        public async Task<Site> UpdateSiteAsync(Guid id, SiteDto dto)
        {
            var site = await _context.Sites.FirstOrDefaultAsync(s => s.Id == id);
            if (site == null)
                throw new Exception("Sitio no encontrado");

            site.Name = dto.Name;
            site.Domain = dto.Domain;
            site.Url = dto.Url;
            site.Color = dto.Color;
            site.IsMaintenance = dto.IsMaintenance;
            site.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return site;
        }

        public async Task DeleteSiteAsync(Guid id)
        {
            var site = await _context.Sites.FirstOrDefaultAsync(s => s.Id == id);
            if (site == null)
                throw new Exception("Sitio no encontrado");

            _context.Sites.Remove(site);
            await _context.SaveChangesAsync();
        }

        public async Task<SiteAccessDto?> GetUserAccessAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserSites)
                .ThenInclude(us => us.SiteNavigate)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            return new SiteAccessDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                AllowedSites = user.UserSites.Select(us => new SiteDto
                {
                    Id = us.SiteNavigate.Id,
                    Name = us.SiteNavigate.Name,
                    Domain = us.SiteNavigate.Domain,
                    Url = us.SiteNavigate.Url,
                    Color = us.SiteNavigate.Color,
                    IsMaintenance = us.SiteNavigate.IsMaintenance,
                    IsActive = us.SiteNavigate.IsActive,
                    CreatedAt = us.SiteNavigate.CreatedAt
                }).ToList()
            };
        }
    }
}


