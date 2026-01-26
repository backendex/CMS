namespace CMS.src.Domain.Entities
{
    public class PostTranslation
    {
        public Guid Id { get; private set; }
        public Guid PostId { get; private set; }
        public string Language { get; private set; } = null!;
        public string Title { get; private set; } = null!;
        public string Slug { get; private set; } = null!;
        public string Content { get; private set; } = null!;
        public string SeoTitle { get; private set; } = null!;
        public string SeoDescription { get; private set; } = null!;


        protected PostTranslation() { }


        public PostTranslation(Guid postId, string language, string title, string slug, string content)
        {
            Id = Guid.NewGuid();
            PostId = postId;
            Language = language;
            Title = title;
            Slug = slug;
            Content = content;
            SeoTitle = title;
            SeoDescription = content.Length > 160 ? content[..160] : content;
        }
    }
}
