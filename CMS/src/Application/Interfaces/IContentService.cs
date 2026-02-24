using CMS.src.Application.DTOs.Content;
using CMS.src.Domain.Entities;

namespace CMS.src.Application.Interfaces

{
    public interface  IContentService
    {
        Task<bool> UpdateBulkContentAsync(List<ContentUpdateDto> contentUpdate);
        Task<List<SiteContent>> GetContentBySiteIdAsync(Guid siteId);
        Task<IEnumerable<MediaContent>> GetMediaBySiteAsync(Guid siteId);
        Task<MediaContent> SaveMediaAsync(MediaContent media);
        Task<long> CreatePostAsync(BlogPost blogDto);
        Task<BlogPost?> GetPostByIdAsync(long id, Guid siteId);
        Task<bool> ExistsBySlugAsync(string slug, Guid siteId);
        Task<IEnumerable<BlogPost>> GetPostsAsync(Guid siteId);
        Task<IEnumerable<Category>> GetCategoriesAsync(Guid siteId);
        Task<Guid> CreateCategoryAsync(CategoryDto categoryDto);
        Task UpdatePostAsync(BlogPost blogDto);
    }
}
