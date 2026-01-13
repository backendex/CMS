using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Domain.Entities
{
    public class User
    {
        //Estructura tal cual se define en la base de datos
        [Column("id")]
        public int Id { get; set; }
        [Column("user_name")]
        public string UserName { get; set; }
        [Column("name")]
        public string Name { get; init; }
        [Column("last_name")]
        public string LastName { get; init; }
        [Column("email")]
        public string Email { get; set; } = string.Empty;
        [Column("password")]
        public string Password { get; set; } = string.Empty;
        [Column("roi_id")]
        public int RolId { get; set; }
        [Column("full_name")]
        public string FullName{ get; set; } = null!;
        public bool IsTemporaryPassword { get; set; } = true;
        [Column("password_hash")]
        public string? PasswordHash { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }
        public string? ValidationToken { get; set; }
        [Column("email_confirmed")]
        public bool EmailConfirmed { get; set; }

        [ForeignKey("RolId")]
        public virtual AccessRole AccessRole { get; set; } = null!;

    }
}

