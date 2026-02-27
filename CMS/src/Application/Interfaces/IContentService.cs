using CMS.src.Application.DTOs.Content;
using CMS.src.Domain.Entities;

namespace CMS.src.Application.Interfaces

{
    public interface  IContentService
    {
        Task<IEnumerable<BlogPost>> GetPostAsync(string siteName, Guid siteId);
        Task<BlogPost?> GetPostByIdAsync(string siteName, long id, Guid siteId);
        Task<IEnumerable<Category>> GetCategoriesAsync(Guid siteId, string siteName);
        Task UpdatePostAsync(BlogPost blogDto, string siteName);
        Task<long> CreatePostAsync(BlogPost blogPost, string siteName);
        Task<Guid> CreateCategoryAsync(CategoryDto categoryDto, string siteName);
        Task<IEnumerable<MediaContent>> GetMediaBySiteAsync(Guid siteId, string siteName);
        Task<MediaContent> SaveMediaAsync(MediaContent media, string siteName);
    }
}
