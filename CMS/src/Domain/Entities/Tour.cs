using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Domain.Entities
{
    [Table("tour")] 
    public class Tour
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("siteid")]
        public Guid SiteId { get; set; }

        [Required]
        [Column("name")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("price")]
        public decimal Price { get; set; }

        [Column("category")]
        public string Category { get; set; } = "General"; 

        [Column("isactive")]
        public bool IsActive { get; set; } = true;

        [Column("seotitle")]
        public string SeoTitle { get; set; } = string.Empty;

        [Column("seodescription")]
        public string SeoDescription { get; set; } = string.Empty;

        [Column("slug")]
        public string Slug { get; set; } = string.Empty; 
    }
}
