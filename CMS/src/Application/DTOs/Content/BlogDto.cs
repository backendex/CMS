using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Application.DTOs.Content
{
    public class BlogDto
    {
        [Required(ErrorMessage = "El título es obligatorio")]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Slug { get; set; }

        [Required]
        public string Content { get; set; }

        public string FeaturedImage { get; set; }

        [Required]
        [Column("site_id")]
        public Guid SiteId { get; set; }

        public SeoMetadataDto SeoData { get; set; }

    }
}
