namespace CMS.src.Application.DTOs.Auth
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string Token { get; set; } 
        public string Message { get; set; }
        public bool MustChangePassword { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }
}
