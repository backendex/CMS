namespace CMS.src.Application.DTOs.Auth
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; } 
        public IEnumerable<string>? Errors { get; set; } 
    }
}
