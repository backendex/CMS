using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Domain.Entities
{
    public class AccessRole 
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
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
