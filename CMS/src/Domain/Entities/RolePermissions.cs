using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Security;

namespace CMS.src.Domain.Entities
{
    [Table("role_permissions")]
    public class RolePermissions
    {
        [Column("role_id")]
        public int RoleId { get; set; }
        public AccessRole Rol { get; set; }

        [Column("permission_id")]
        public int PermissionId { get; set; }
        public Permissions Permissions { get; set; }
    }
}
