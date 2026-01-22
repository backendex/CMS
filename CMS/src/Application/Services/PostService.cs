using CMS.src.Application.DTOs.Post;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using CMS.src.Infrastructure.Persistence.Repositories;

namespace CMS.src.Application.Services
{
    public class PostService
    {
        private readonly IPostService _postService;

        public PostService(IPostService repository)
        {
            _postService = repository;
        }

        public async Task CreateAsync(CreatePostDto dto, Guid authorId)
        {
            var post = new Post(dto.Title, dto.Content, authorId);
            await _postService.AddAsync(post);
            await _postService.SaveChangesAsync();
        }

        public async Task PublishAsync(Guid postId)
        {
            var post = await _postService.GetByIdAsync(postId);
            if (post is null)
                throw new Exception("Post no encontrado");

            post.Publish();
            await _postService.SaveChangesAsync();
        }

        public async Task<List<PostResponseDto>> GetAllAsync()
        {
            var posts = await _postService.GetAllAsync();

            return posts.Select(p => new PostResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                Status = p.Status,
                CreatedAt = p.CreatedAt
            }).ToList();
        }
    }

}
