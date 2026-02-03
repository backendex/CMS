using CMS.Infrastructure.Persistence;
using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using CMS.src.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

namespace CMS.src.Application.Services
{
    public class SiteService : ISiteService
    {
        private readonly ApplicationDbContext _context;

        public SiteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Site> GetByDomainAsync(string domain)
        {
            throw new NotImplementedException();
        }

        public async Task<SiteAccessDto?> GetUserAccessAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserSites)
                .ThenInclude(us => us.Site)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            return new SiteAccessDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                AllowedSites = user.UserSites.Select(us => new SiteDto
                {
                    Id = us.Site.Id,
                    Name = us.Site.Name,
                    Domain = us.Site.Domain,
                    IsActive = us.Site.IsActive,
                    CreatedAt = us.Site.CreatedAt
                }).ToList()
            };
        }
    }


}


