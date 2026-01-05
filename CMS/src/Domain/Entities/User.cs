using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.AccessControl;

namespace CMS.src.Domain.Entities
{
    // Entities/User.cs
    public class User
    {
        //Estructura tal cual se define en la base de datos
        [Column("id")]
        public int Id { get; set; }
        [Column("email")]
        public string Email { get; set; } = string.Empty;
        [Column("password")]
        public string Password { get; set; } = string.Empty;
        [Column("roi_id")]
        public int RolId { get; set; }
        public AccessRole Role { get; set; } = null!;

        // Nuevos campos necesarios para la validación
        [Column("is_active")]
        public bool IsActive { get; set; } = false;
        public string? ValidationToken { get; set; }
    }
}
