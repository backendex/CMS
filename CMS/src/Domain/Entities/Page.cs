namespace CMS.src.Domain.Entities
{
    public class Page
    {
        public Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public string Slug { get; set; } = null!;
        public bool IsPublished { get; set; }

        public ICollection<PageTranslation> Translations { get; set; } = new List<PageTranslation>();

        public Page() { }

        public Page(Guid siteId, string slug)
        {
            Id = Guid.NewGuid();
            SiteId = siteId;
            Slug = slug;
        }

        public void Publish() => IsPublished = true;
    }
}
