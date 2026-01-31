using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Application.DTOs.Content
{
    public class ContentUpdateDto
    {
        public Guid? Id { get; set; }
        public Guid SiteId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}