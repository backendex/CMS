using CMS.src.Domain.Entities;

namespace CMS.src.Application.DTOs.Post
{
    public class UpdatePostDto
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}

