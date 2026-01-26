namespace CMS.src.Domain.Entities
{
    public class Page
    {
        public Guid Id { get; private set; }
        public Guid SiteId { get; private set; }
        public string Slug { get; private set; } = null!;
        public bool IsPublished { get; private set; }


        public ICollection<PageTranslation> Translations { get; private set; } = new List<PageTranslation>();


        protected Page() { }


        public Page(Guid siteId, string slug)
        {
            Id = Guid.NewGuid();
            SiteId = siteId;
            Slug = slug;
        }


        public void Publish() => IsPublished = true;
    }
}
