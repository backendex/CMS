using CMS.src.Application.DTOs.Content;
using CMS.src.Domain.Entities;

namespace CMS.src.Application.Interfaces

{
    public interface IContentService
    {
        Task<IEnumerable<BlogPost>> GetPostAsync(string TableName, Guid siteId);
        Task<BlogPost?> GetPostBySiteIdAsync(string TableName, long id);
        Task<IEnumerable<Category>> GetCategoriesAsync(Guid siteId, string siteName);
        Task UpdatePostAsync(BlogPost blogDto, string TableName);
        Task DeletePostAsync(long id, string TableName);
        Task<long> CreatePostAsync(BlogPost blogPost, string TableName);
        Task<Guid> CreateCategoryAsync(CategoryDto categoryDto, string siteName);
        Task<IEnumerable<MediaContent>> GetMediaBySiteAsync(Guid siteId, string siteName);
        Task<MediaContent> SaveMediaAsync(MediaContent media, string siteName);
        string TableName { get; }
    }
}