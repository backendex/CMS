using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Domain.Entities
{
    [Table("sites")]
    public class Site
    {
        [Column("id")]
        public Guid Id { get; private set; }
        [Column("name")]
        public string Name { get; private set; } = null!;
        [Column("domain")]
        public string Domain { get; private set; } = null!;
        [Column("is_active")]
        public bool IsActive { get; private set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;


    }
}
