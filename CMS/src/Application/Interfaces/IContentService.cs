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
        Task<Guid> CreatePostAsync(BlogPost blogDto);
        Task<bool> ExistsBySlugAsync(string slug, Guid siteId);
        Task<List<BlogPost>> GetPostsAsync(string? siteId = null);


    }
}
