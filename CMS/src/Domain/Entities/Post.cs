using CMS.src.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

namespace CMS.Domain.Entities
{
    public class Post
    {
        public Guid Id { get; private set; }
        public Guid SiteId { get; private set; }
        public int AuthorId { get; private set; }
        public PostStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public User Author { get; private set; } = null!;

        private readonly List<PostTranslation> _translations = new();
        public IReadOnlyCollection<PostTranslation> Translations => _translations.AsReadOnly();

        protected Post() { }

        public Post(Guid siteId, int authorId)
        {
            Id = Guid.NewGuid();
            SiteId = siteId;
            AuthorId = authorId;
            Status = PostStatus.Draft;
            CreatedAt = DateTime.UtcNow;
        }

        public void Publish()
        {
            Status = PostStatus.Published;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddTranslation(PostTranslation translation)
        {
            _translations.Add(translation);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public enum PostStatus
    {
        Draft = 0,
        Published = 1
    }
}

