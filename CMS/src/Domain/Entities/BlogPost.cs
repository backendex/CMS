using CMS.src.Domain.Entities;
using Microsoft.Data.SqlClient.Server;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CMS.src.Domain.Entities
{
    [Table("blog_post")]
    public class BlogPost
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [Column("site_id")]
        public Guid SiteId { get; set; }
        [Required]
        [MaxLength(200)]
        [Column("title")]
        public string Title { get; set; }
        [Required]
        [MaxLength(250)]
        [Column("slug")]
        public string Slug { get; set; } 
        [Required]
        [Column("content")]
        public string Content { get; set; }
        [Column("featured_image")]
        public string FeaturedImage { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        [Column("is_published")]
        public bool IsPublished { get; set; }
        [Column("seo_data", TypeName = "jsonb")]
        public SeoMetadata SeoData { get; set; } = new SeoMetadata();

    }
}

