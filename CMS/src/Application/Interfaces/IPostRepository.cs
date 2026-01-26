using CMS.Domain.Entities;

namespace CMS.src.Application.Interfaces
{
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(Guid id);
        Task<IEnumerable<Post>> GetPublishedBySiteAsync(Guid siteId);
        Task AddAsync(Post post);
        Task SaveChangesAsync();
    }
}

