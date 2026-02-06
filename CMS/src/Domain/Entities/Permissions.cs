using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Domain.Entities
{
    [Table("permissions")]
    public class Permissions
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("permission_key")]
        public string PermissionKey { get; set; } 
        [Column("description")]
        public string Description { get; set; }
        public ICollection<RolePermissions> RolePermissions { get; set; }
    }
}
