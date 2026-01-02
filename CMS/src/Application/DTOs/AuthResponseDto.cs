namespace CMS.src.Application.DTOs
{
    public class AuthResponseDto
    {
        // Indica si la operación (Login/Registro) fue exitosa
        public bool IsSuccess { get; set; }

        // Mensaje para mostrar al usuario (ej: "Contraseña incorrecta" o "Usuario creado")
        public string Message { get; set; } = string.Empty;

        // El Token JWT que React guardará en el LocalStorage o Cookies
        public string Token { get; set; } = string.Empty;

        // Datos básicos para mostrar en el perfil del Dashboard sin decodificar el token
        public string Email { get; set; } = string.Empty;

        // El rol principal para la lógica de redirección inicial en el frontend
        public string Role { get; set; } = string.Empty;
    }
}
