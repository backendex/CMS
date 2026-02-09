using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Domain.Entities
{
    [Table("media")]
    public class MediaContent
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("site_id")]
        public Guid SiteId { get; set; }
        [Column("url")]
        public string Url { get; set; } = string.Empty;
        [Column("alt_text")]
        public string AltText { get; set; } = string.Empty;
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
