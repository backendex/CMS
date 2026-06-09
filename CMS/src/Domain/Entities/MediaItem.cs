using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Domain.Entities
{
    [Table("media_items")]
    public class MediaItem
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("url")]
        public string Url { get; set; }
        [Column("file_id")]
        public string FileId { get; set; }
        [Column("file_name")]
        public string FileName { get; set; }
        [Column("file_type")]
        public string FileType { get; set; }
        [Column("site_id")]
        public string SiteId { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
