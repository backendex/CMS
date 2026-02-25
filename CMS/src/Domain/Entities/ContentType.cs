using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace CMS.src.Domain.Entities
{
    [Table("content_type")]
    public class ContentType
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("siteid")]
        public Guid SiteId { get; set; }
        [Column(TypeName = " jsonb")]
        public JsonDocument Schema { get; set; }
    }
}
