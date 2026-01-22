using CMS.src.Domain.Entities;
using System.Threading.Tasks;

namespace CMS.src.Application.Interfaces
{
    public interface IPostService
    {
        Task AddAsync(Post post);
        Task<Post?> GetByIdAsync(Guid id);
        Task<List<Post>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
