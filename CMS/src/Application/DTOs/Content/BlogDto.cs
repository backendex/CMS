using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Application.DTOs.Content
{
    public class BlogDto
    {
        public long Id { get; set; }
        public Guid SiteId { get; set; }

        // Mapeado de post_title
        public string Title { get; set; } = string.Empty;

        // Mapeado de post_name (el antiguo Slug)
        public string Slug { get; set; } = string.Empty;

        // Mapeado de post_content
        public string Content { get; set; } = string.Empty;

        // Mapeado de post_excerpt (resumen corto)
        public string Excerpt { get; set; } = string.Empty;

        // Mapeado de post_status (ej: 'publish', 'draft')
        public string Status { get; set; } = "publish";

        // Mapeado de post_date (como es TEXT en DB, aquí es string)
        public string PublishedAt { get; set; } = string.Empty;

        // Para el frontend sigue siendo útil saber si está publicado
        public bool IsPublished => Status.Equals("publish", StringComparison.OrdinalIgnoreCase);

        // Mapeado de guid (útil para links permanentes)
        public string Url { get; set; } = string.Empty;

        // Campos adicionales para UI
        public long AuthorId { get; set; }
        public long CommentCount { get; set; }

        public SeoMetadataDto SeoData { get; set; }

    }
}
