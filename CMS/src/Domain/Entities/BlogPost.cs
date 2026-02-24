using CMS.src.Domain.Entities;
using Microsoft.Data.SqlClient.Server;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CMS.src.Domain.Entities
{
    [Table("wp_posts")]
    public class BlogPost
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("site_id")]
        public Guid SiteId { get; set; }

        [Column("post_author")]
        public long PostAuthor { get; set; } = 0;

        [Required]
        [Column("post_date")]
        public string PostDate { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        [Column("post_date_gmt")]
        public string PostDateGmt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        [Required]
        [Column("post_content")]
        public string PostContent { get; set; }

        [Required]
        [Column("post_title")]
        public string PostTitle { get; set; }

        [Column("post_excerpt")]
        public string PostExcerpt { get; set; } = string.Empty;

        [Column("post_status")]
        [MaxLength(20)]
        public string PostStatus { get; set; } = "publish";

        [Column("comment_status")]
        [MaxLength(20)]
        public string CommentStatus { get; set; } = "open";

        [Column("ping_status")]
        [MaxLength(20)]
        public string PingStatus { get; set; } = "open";

        [Column("post_password")]
        [MaxLength(255)]
        public string PostPassword { get; set; } = string.Empty;

        [Column("post_name")]
        [MaxLength(200)]
        public string PostName { get; set; }

        [Column("to_ping")]
        public string ToPing { get; set; } = string.Empty;

        [Column("pinged")]
        public string Pinged { get; set; } = string.Empty;

        [Column("post_modified")]
        public string PostModified { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        [Column("post_modified_gmt")]
        public string PostModifiedGmt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        [Column("post_content_filtered")]
        public string PostContentFiltered { get; set; } = string.Empty;

        [Column("post_parent")]
        public long PostParent { get; set; } = 0;

        [Column("guid")]
        [MaxLength(255)]
        public string Guid { get; set; } = string.Empty;

        [Column("menu_order")]
        public int MenuOrder { get; set; } = 0;

        [Column("post_type")]
        [MaxLength(20)]
        public string PostType { get; set; } = "post";

        [Column("post_mime_type")]
        [MaxLength(100)]
        public string PostMimeType { get; set; } = string.Empty;

        [Column("comment_count")]
        public long CommentCount { get; set; } = 0;

        [Column("seo_data", TypeName = "jsonb")]
        public SeoMetadata SeoData { get; set; } = new SeoMetadata();


    }
}

