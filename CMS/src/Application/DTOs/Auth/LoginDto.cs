namespace CMS.src.Application.DTOs.Auth
{
    public class LoginDto
    {
        public string Email { get; init; }
        public string Password { get; init; }
        public int RolId { get; init; } = 0;
        public string RolName { get; init; } 
    }
}
