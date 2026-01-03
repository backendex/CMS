using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Domain.Entities
{
    [Table("users")] // Fuerza el nombre de la tabla a minúsculas como en tu DB
    public class User
    {
        [Key]
        [Column("id")] // Mapea a la columna 'id' de tu imagen
        public int Id { get; set; }

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("password")]
        public string Password { get; set; } = string.Empty;

        [Column("rol_id")] // Mapea a 'rol_id' de tu imagen
        public int RolId { get; set; }

        // Si quieres mantener estas propiedades en C# pero no están en la DB, 
        // debes marcarlas como [NotMapped] o agregarlas a la tabla en Postgres.
        [NotMapped]
        public string FirstName { get; set; } = string.Empty;
        [NotMapped]
        public string LastName { get; set; } = string.Empty;
    }
}
