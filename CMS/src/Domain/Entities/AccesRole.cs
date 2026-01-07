using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Domain.Entities
{
    public class AccessRole : IdentityRole<int>
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name_rol")]
        public string NameRol { get; set; } = string.Empty;
        [Column("can_view")]
        public bool CanView { get; set; }
        [Column("can_create")]
        public bool CanCreate { get; set; }
        [Column("can_edit")]
        public bool CanEdit { get; set; }
        [Column("can_delete")]
        public bool CanDelete { get; set; }

        //Collection
        // Propiedad de navegación (opcional, para ver qué usuarios tienen este rol)
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
