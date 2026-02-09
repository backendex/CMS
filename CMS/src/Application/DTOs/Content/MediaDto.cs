using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Application.DTOs.Content
{
    public class MediaDto
    {
        public Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
