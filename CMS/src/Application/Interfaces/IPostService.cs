using CMS.src.Application.DTOs.Post;
using CMS.src.Domain.Entities;
using System.Threading.Tasks;

namespace CMS.src.Application.Interfaces
{
    public interface IPostService
    {
        Task CreateAsync(CreatePostDto dto, int authorId);
        Task PublishAsync(Guid postId);
    }
}


