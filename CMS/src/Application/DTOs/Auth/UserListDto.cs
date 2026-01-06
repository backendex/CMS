namespace CMS.src.Application.DTOs.Auth
{
    public class UserListDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // Nombre del rol (Admin, Editor, etc.)
        public bool EmailConfirmed { get; set; } // Para saber si ya activó su cuenta
        public DateTime CreatedAt { get; set; }
    }
}
