namespace CMS.src.Application.DTOs.Auth
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? Token { get; set; } 
        public string? Email { get; set; }

        // Constructor para respuestas exitosas+¿+
                public AuthResponse(bool success, string message, string? token = null)
        {
            Success = success;
            Message = message;
            Token = token;
    }   }    
}





