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
        public async Task<IEnumerable<MediaContent>> GetMediaBySiteAsync(Guid siteId)
        {
            return await _context.Media
                .Where(m => m.SiteId == siteId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<MediaContent> SaveMediaAsync(MediaContent media)
        {
            media.Id = Guid.NewGuid();
            media.CreatedAt = DateTime.UtcNow;
            _context.Media.Add(media);
            await _context.SaveChangesAsync();
            return media;
        }
        public async Task<Guid> CreatePostAsync(BlogPost blogDto)
        {
            if (await ExistsBySlugAsync(blogDto.Slug, blogDto.SiteId))
            {
                throw new Exception("El slug ya existe para este sitio.");
            }

            if (blogDto.Id == Guid.Empty)
            {
                blogDto.Id = Guid.NewGuid();
            }

            _context.BlogPost.Add(blogDto);
            await _context.SaveChangesAsync();

            return blogDto.Id;
        }

        public async Task<bool> ExistsBySlugAsync(string slug, Guid siteId)
        {
            return await _context.BlogPost
                .AnyAsync(b => b.Slug == slug && b.SiteId == siteId);
        }
        public async Task UpdatePostAsync(BlogPost blogDto)
        {
            var existingPost = await _context.BlogPost
                .FirstOrDefaultAsync(b => b.Id == blogDto.Id);

            if (existingPost == null)
            {
                throw new Exception("El blog post no fue encontrado.");
            }

            existingPost.Title = blogDto.Title;
            existingPost.Slug = blogDto.Slug;
            existingPost.Content = blogDto.Content;
            existingPost.FeaturedImage = blogDto.FeaturedImage;
            existingPost.IsPublished = blogDto.IsPublished;
            existingPost.UpdatedAt = DateTime.UtcNow;
            existingPost.SeoData = blogDto.SeoData;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var errorDetail = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Error al actualizar en la base de datos: {errorDetail}");
            }
        }
        public async Task<IEnumerable<BlogPost>> GetPostsAsync(Guid siteId)
        {
            return await _context.BlogPost
                .Where(p => p.SiteId == siteId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
        public async Task<BlogPost?> GetPostByIdAsync(Guid id, Guid siteId)
        {
            return await _context.BlogPost
                .FirstOrDefaultAsync(b => b.Id == id && b.SiteId == siteId);
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync(Guid siteId)
        {
            return await _context.Categories
                .Where(c => c.SiteId == siteId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
        public async Task<Guid> CreateCategoryAsync(CategoryDto categoryDto)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = categoryDto.Name,
                Slug = categoryDto.Slug,
                Description = categoryDto.Description,
                SiteId = categoryDto.SiteId,
                ParentCategoryId = categoryDto.ParentCategoryId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category.Id;
        }

    }
}


    
