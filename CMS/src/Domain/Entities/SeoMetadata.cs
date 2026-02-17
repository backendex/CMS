namespace CMS.src.Domain.Entities
{
    public class SeoMetadata
    {
        public string SeoTitle { get; set; }
        public string MetaDescription { get; set; }
        public string FocusKeyword { get; set; }
        public string OgTitle { get; set; }
        public string OgDescription { get; set; }
        public string OgImage { get; set; }
        public string CanonicalUrl { get; set; }
        public string RobotsContent { get; set; } = "index, follow";
    }
}
