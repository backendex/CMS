using CMS.Infrastructure.Persistence;
using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CMS.src.Application.Services
{
    public class ContentService : IContentService
    {
        private readonly ApplicationDbContext _context;

        public ContentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SiteContent>> GetContentBySiteIdAsync(Guid siteId)
        {
            return await _context.SiteContents
                .Where(c => c.SiteId == siteId)
                .ToListAsync();
        }

        public async Task<bool> UpdateBulkContentAsync(List<ContentUpdateDto> contentUpdate)
        {
            foreach (var contentBulk in contentUpdate)
            {
                var entry = await _context.SiteContents
                    .FirstOrDefaultAsync(pt => pt.SiteId == contentBulk.SiteId && pt.Key == contentBulk.Key);

                if (entry != null)
                {
                    entry.Value = contentBulk.Value;
                }
                else
                {
                    _context.SiteContents.Add(new SiteContent
                    {
                        Id = Guid.NewGuid(), 
                        SiteId = contentBulk.SiteId,
                        Key = contentBulk.Key,
                        Value = contentBulk.Value
                    });
                }
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
