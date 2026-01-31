using CMS.src.Application.DTOs.Content;
using CMS.src.Domain.Entities;

namespace CMS.src.Application.Interfaces

{
    public interface IContentService
    {
        Task<bool> UpdateBulkContentAsync(List<ContentUpdateDto> contentUpdate);
        Task<List<SiteContent>> GetContentBySiteIdAsync(Guid siteId);
    }
}
