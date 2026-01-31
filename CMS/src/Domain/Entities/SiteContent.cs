using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Domain.Entities
{
    public class SiteContent
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("siteid")]
        public Guid SiteId { get; set; }

        [Column("key")]
        [MaxLength(100)]
        [Required]
        public string Key { get; set; }
        [Column("value")]
        public string Value { get; set; }
    }
}
