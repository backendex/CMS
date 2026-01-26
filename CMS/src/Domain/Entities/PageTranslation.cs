namespace CMS.src.Domain.Entities
{
    public class PageTranslation
    {
        public Guid Id { get; private set; }
        public Guid PageId { get; private set; }
        public string Language { get; private set; } = null!;
        public string Title { get; private set; } = null!;
        public string BlocksJson { get; private set; } = null!;


        protected PageTranslation() { }


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
