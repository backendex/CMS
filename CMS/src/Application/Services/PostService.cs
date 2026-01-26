using CMS.Domain.Entities;
using CMS.Infrastructure.Persistence;
using CMS.src.Application.DTOs.Post;
using CMS.src.Application.Interfaces;
using CMS.src.Domain.Entities;
using CMS.src.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;
using System;

namespace CMS.src.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task CreateAsync(CreatePostDto dto, int authorId)
        {
            var post = new Post(dto.SiteId, authorId);

            foreach (var t in dto.Translations)
            {
                post.AddTranslation(
                    new PostTranslation(
                        post.Id,
                        t.Language,
                        t.Slug,
                        t.Title,
                        t.Content
                    )
                );
            }


            await _postRepository.AddAsync(post);
            await _postRepository.SaveChangesAsync();
        }

        public async Task PublishAsync(Guid postId)
        {
            var post = await _postRepository.GetByIdAsync(postId)
                ?? throw new Exception("Post not found");
    

            post.Publish();
            await _postRepository.SaveChangesAsync();
        }
    }


}
