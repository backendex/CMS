namespace CMS.src.Application.DTOs
{
    public class UserRegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Para asignar el rol inicial (ej: "Admin", "Editor", "Viewer")
        public string Role { get; set; } = "Viewer";
    }
}
