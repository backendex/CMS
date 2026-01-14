using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Application.DTOs.Auth
{
    public record RegisterDto
    {
        [Column("name")]
        public string Name { get; init; }
        [Column("last_name")]
        public string LastName { get; init; }
        [Column("email")]
        public string Email { get; init; }
        [Column("rol_id")]
        public int RolId { get; init; }
        [Column("email_confirmed")]
        public bool EmailConfirmed { get; set; }

    }
}

