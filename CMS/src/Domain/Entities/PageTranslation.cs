namespace CMS.src.Domain.Entities
{
    public class PageTranslation
    {
        public Guid Id { get; set; }
        public Guid PageId { get; set; }
        public string Language { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string BlocksJson { get; set; } = null!;

        public PageTranslation() { }

        public PageTranslation(Guid pageId, string language, string title, string blocksJson)
        {
            Id = Guid.NewGuid();
            PageId = pageId;
            Language = language;
            Title = title;
            BlocksJson = blocksJson;
        }
    }
}
