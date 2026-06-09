using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Domain.Entities
{
    [Table("sites")]
    public class Site
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("name")]
        public string Name { get; set; } = null!;
        [Column("domain")]
        public string Domain { get; set; } = null!;
        [Column("url")]
        public string? Url { get; set; }
        [Column("color")]
        public string? Color { get; set; }
        [Column("is_maintenance")]
        public bool IsMaintenance { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("table_name")]
        public string? TableName { get; set; }
    }
}
