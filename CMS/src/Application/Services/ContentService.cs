using CMS.Infrastructure.Persistence;
using CMS.src.Application.DTOs.Content;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using CMS.src.Infrastructure.Persistence.Interceptors;
using JWT.Algorithms;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CMS.src.Application.Services
{
    public class ContentService : IContentService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly string _connectionString;

        public ContentService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        private ApplicationDbContext CreateContextForSite(string siteName)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(_connectionString)
                          .AddInterceptors(new DynamicTableInterceptor($"wp_{siteName.ToLower()}"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
        public async Task<IEnumerable<BlogPost>> GetPostAsync(string siteName, Guid siteId)
        {
            using var context = CreateContextForSite(siteName);
            return await context.BlogPost
                .Where(p => p.SiteId == siteId)
                .OrderByDescending(p => p.PostDate)
                .ToListAsync();

        }
        public async Task<List<SiteContent>> GetContentBySiteIdAsync(Guid siteId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.SiteContents
                .Where(c => c.SiteId == siteId)
                .ToListAsync();
        }
        //Busqueda de post por id
        public async Task<BlogPost?> GetPostByIdAsync(string siteName,long id, Guid siteId)
        {
            using var context = CreateContextForSite(siteName);
            return await context.BlogPost
                .FirstOrDefaultAsync(b => b.Id == id && b.SiteId == siteId);
        }
        public async Task<List<SiteContent>> GetPostBySiteIdAsync(Guid siteId, string siteName)
        {
            // 1. Crear el contexto usando la factoría y el interceptor
            using var context = CreateContextForSite(siteName);

            // 2. Ahora sí puedes acceder a SiteContents
            return await context.SiteContents
                .Where(c => c.SiteId == siteId)
                .ToListAsync();
        }
        //Busqueda de categorias
        public async Task<IEnumerable<Category>> GetCategoriesAsync(Guid siteId, string siteName)
        {
            using var context = CreateContextForSite(siteName);
            return await context.Categories
                .Where(c => c.SiteId == siteId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
        //Actualizar
        public async Task UpdatePostAsync(BlogPost blogDto, string siteName)
        {
            using var context = CreateContextForSite(siteName);
            var existingPost = await context.BlogPost
                .FirstOrDefaultAsync(b => b.Id == blogDto.Id);

            if (existingPost == null) throw new Exception("El blogs post no se encontro");

            existingPost.PostTitle = blogDto.PostTitle;
            existingPost.PostName = blogDto.PostName;
            existingPost.PostContent = blogDto.PostContent;
            existingPost.PostStatus = blogDto.PostStatus;
            string now = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            existingPost.PostModified = now;
            existingPost.PostModifiedGmt = now;
            existingPost.PostModified = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            existingPost.SeoData = blogDto.SeoData;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var errorDetail = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Error al actualizar en la base de datos: {errorDetail}");
            }
        }
        public async Task<long> CreatePostAsync(BlogPost blogPost, string siteName)
        {
            using var context = CreateContextForSite(siteName);

            // Validar duplicados de slug antes de insertar
            if (await context.BlogPost.AnyAsync(b => b.PostName == blogPost.PostName && b.SiteId == blogPost.SiteId))
            {
                throw new Exception("El slug ya existe para este sitio.");
            }

            context.BlogPost.Add(blogPost);
            await context.SaveChangesAsync();
            return blogPost.Id;
        }
        //Crear categoria
        public async Task<Guid> CreateCategoryAsync(CategoryDto categoryDto, string siteName)
        {
            using var context = CreateContextForSite(siteName);
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
        //Busqueda para media
        public async Task<IEnumerable<MediaContent>> GetMediaBySiteAsync(Guid siteId, string siteName)
        {
            using var context = CreateContextForSite(siteName);
            return await context.Media
                .Where(m => m.SiteId == siteId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }
        //se guarda media
        public async Task<MediaContent> SaveMediaAsync(MediaContent media, string siteName)
        {
            using var context = CreateContextForSite(siteName);
            media.Id = Guid.NewGuid();
            media.CreatedAt = DateTime.UtcNow;
            context.Media.Add(media);
            await context.SaveChangesAsync();
            return media;
        }

    }
}


    
