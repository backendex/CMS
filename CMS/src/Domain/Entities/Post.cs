namespace CMS.src.Domain.Entities
{
    public class Post
    {
        public Guid Id { get; private set; }

        public string Title { get; private set; } = null!;
        public string Content { get; private set; } = null!;
        public string Status { get; private set; } = "DRAFT";

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public Guid AuthorId { get; private set; }

        private Post() { }

        public Post(string title, string content, Guid authorId)
        {
            Id = Guid.NewGuid();
            Title = title;
            Content = content;
            AuthorId = authorId;
            CreatedAt = DateTime.UtcNow;
        }

        public void Publish()
        {
            Status = "PUBLISHED";
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(string title, string content)
        {
            Title = title;
            Content = content;
            UpdatedAt = DateTime.UtcNow;
        }
    }


}
