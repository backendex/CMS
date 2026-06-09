using CMS.Infrastructure.Persistence;
using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.src.Application.Services
{
    public class ContentService : IContentService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IApplicationDbContext _context;

        public ContentService(IDbContextFactory<ApplicationDbContext> contextFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _contextFactory = contextFactory;
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no fue encontrada.");
            _httpContextAccessor = httpContextAccessor;

        }
        private async Task<ApplicationDbContext> CreateContextForSite(string TableName)
        {
            var context = await _contextFactory.CreateDbContextAsync();

            string cleanName = TableName.Trim().ToLower();

            if (!cleanName.StartsWith("wp_"))
            {
                cleanName = $"wp_{cleanName}";
            }

            context.CurrentTableName = cleanName;

            return context;
        }
        public async Task<IEnumerable<BlogPost>> GetPostAsync(string TableName,Guid siteId)
        {
            await using var context = await CreateContextForSite(TableName);

            return await context.BlogPost
                .Where(p => p.SiteId == siteId)
                .OrderByDescending(p => p.PostDate)
                .ToListAsync();
        }
        public async Task<BlogPost?> GetPostBySiteIdAsync(string TableName, long id)
        {
            await using var context = await CreateContextForSite(TableName);

            return await context.BlogPost
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        
        public async Task<long> CreatePostAsync(BlogPost blogPost, string siteName)
        {
            await using var context = await CreateContextForSite(siteName);

            if (await context.BlogPost
                .AnyAsync(b => b.PostName == blogPost.PostName && b.SiteId == blogPost.SiteId))
            {
                throw new Exception("El slug ya existe para este sitio.");
            }

            context.BlogPost.Add(blogPost);
            await context.SaveChangesAsync();

            return blogPost.Id;
        }
        public async Task UpdatePostAsync(BlogPost blogPost, string siteName)
        {

            if (string.IsNullOrEmpty(siteName)) throw new ArgumentException("El nombre del sitio es requerido.");

            await using var context = await CreateContextForSite(siteName);

            var existingPost = await context.BlogPost
                .FirstOrDefaultAsync(b => b.Id == blogPost.Id);

            if (existingPost == null)
                throw new Exception($"El blog post con ID {blogPost.Id} no se encontró en {siteName}.");

            existingPost.PostTitle = blogPost.PostTitle;
            existingPost.PostName = blogPost.PostName;
            existingPost.PostContent = blogPost.PostContent;
            existingPost.PostStatus = blogPost.PostStatus;
            existingPost.PostExcerpt = blogPost.PostExcerpt;
            existingPost.PostMimeType = blogPost.PostMimeType;
            existingPost.SeoData = blogPost.SeoData;

            existingPost.PostModified = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"); 

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fallo en SaveChanges para {siteName}: {ex.Message}", ex);
            }
        }
        public async Task DeletePostAsync(long id, string TableName)
        {
            await using var context = await CreateContextForSite(TableName);

            var post = await context.BlogPost
                .FirstOrDefaultAsync(b => b.Id == id);

            if (post == null)
                throw new Exception($"No se encontró el post con ID {id} en la tabla {TableName}");

            context.BlogPost.Remove(post);
            await context.SaveChangesAsync();
        }
        public async Task<IEnumerable<MediaContent>> GetMediaBySiteAsync(Guid siteId, string siteName)
        {
            await using var context = await CreateContextForSite(siteName);

            return await context.Media
                .Where(m => m.SiteId == siteId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }
        public async Task<MediaContent> SaveMediaAsync(MediaContent media, string siteName)
        {
            await using var context = await CreateContextForSite(siteName);
            context.Media.Add(media);
            await context.SaveChangesAsync();
            return media;
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync(Guid siteId, string siteName)
        {
            await using var context = await CreateContextForSite(siteName);

            return await context.Categories
                .Where(c => c.SiteId == siteId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
        public async Task<Guid> CreateCategoryAsync(CategoryDto categoryDto, string siteName)
        {
            await using var context = await CreateContextForSite(siteName);

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = categoryDto.Name,
                Slug = categoryDto.Slug,
                Description = categoryDto.Description,
                SiteId = categoryDto.SiteId,
                ParentCategoryId = categoryDto.ParentCategoryId
            };

            context.Categories.Add(category);
            await context.SaveChangesAsync();

            return category.Id;
        }
        public string TableName
        {
            get
            {
                var siteName = _httpContextAccessor
                    .HttpContext?
                    .Request
                    .Query["siteName"]
                    .ToString();

                if (string.IsNullOrEmpty(siteName))
                    return "wp_posts";


                return siteName;
            }
        }
    }

}


